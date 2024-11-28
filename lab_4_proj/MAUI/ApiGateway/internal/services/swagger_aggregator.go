package services

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strings"
	"sync"
)

type SwaggerDoc struct {
	Swagger string `json:"swagger,omitempty"`
	OpenAPI string `json:"openapi,omitempty"`
	Info    struct {
		Version string `json:"version"`
		Title   string `json:"title"`
	} `json:"info"`
	Paths       map[string]interface{} `json:"paths"`
	Definitions map[string]interface{} `json:"components"`
}

func FetchSwagger(url string, wg *sync.WaitGroup, docs chan<- SwaggerDoc) {
	defer wg.Done()
	resp, err := http.Get(url)
	if err != nil {
		fmt.Printf("Error fetching Swagger from %s: %v\n", url, err)
		return
	}
	defer resp.Body.Close()

	var doc SwaggerDoc
	if err := json.NewDecoder(resp.Body).Decode(&doc); err != nil {
		fmt.Printf("Error decoding Swagger JSON from %s: %v\n", url, err)
		return
	}

	docs <- doc
}

func AggregateSwagger(serviceURLs []string, mainApiRoute string) SwaggerDoc {
	var wg sync.WaitGroup
	docs := make(chan SwaggerDoc, len(serviceURLs))

	for _, url := range serviceURLs {
		wg.Add(1)
		go FetchSwagger(url, &wg, docs)
	}

	wg.Wait()
	close(docs)

	combined := SwaggerDoc{
		OpenAPI: "3.0.1",
		Info: struct {
			Version string `json:"version"`
			Title   string `json:"title"`
		}{
			Version: "1.0.0",
			Title:   "Car Rental API Documentation",
		},
		Paths:       make(map[string]interface{}),
		Definitions: make(map[string]interface{}),
	}

	for doc := range docs {
		for path, details := range doc.Paths {
			// Remove the old prefix ("/name-of-the-service/api/")
			newPath := path

			if idx := strings.Index(newPath, "/api/"); idx != -1 {
				newPath = newPath[idx+len("/api/")-1:]
			}

			newPath = mainApiRoute + newPath
			combined.Paths[newPath] = details
		}

		for defName, defDetails := range doc.Definitions {
			combined.Definitions[defName] = defDetails
		}
	}

	return combined
}
