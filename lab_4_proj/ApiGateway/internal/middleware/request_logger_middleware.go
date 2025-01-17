package middleware

import (
	"io/ioutil"
	"log"
	"time"

	"bytes"

	"github.com/gin-gonic/gin"
)

func RequestLogger() gin.HandlerFunc {
	return func(c *gin.Context) {
		start := time.Now()

		// Log route parameters
		params := c.Params
		var routeParams string
		for _, param := range params {
			routeParams += param.Key + "=" + param.Value + "&"
		}
		if len(routeParams) > 0 {
			routeParams = routeParams[:len(routeParams)-1] // Remove trailing '&'
		}

		// Log request body
		var requestBody string
		if c.Request.Body != nil {
			bodyBytes, _ := ioutil.ReadAll(c.Request.Body)
			requestBody = string(bodyBytes)
			c.Request.Body = ioutil.NopCloser(bytes.NewBuffer(bodyBytes)) // Reset the body
		}

		c.Next()

		log.Printf("[Request] %s %s | Params: %s | Body: %s | Status: %d | Duration: %s",
			c.Request.Method,
			c.Request.URL.Path,
			routeParams,
			requestBody,
			c.Writer.Status(),
			time.Since(start))
	}
}
