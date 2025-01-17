package services

import (
	"net/http"
	"net/http/httputil"
	"net/url"
	"strings"

	"github.com/gin-gonic/gin"
)

func ReverseProxy(serviceURL string, servicePath string, mainApiRoute string) gin.HandlerFunc {
	return func(c *gin.Context) {
		target, err := url.Parse(serviceURL)
		if err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": "Service unavailable"})
			return
		}
		if strings.HasPrefix(c.Request.URL.Path, mainApiRoute) {
			c.Request.URL.Path = strings.Replace(c.Request.URL.Path, mainApiRoute, servicePath, 1)
		} else {
			c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid request path"})
			return
		}

		proxy := httputil.NewSingleHostReverseProxy(target)
		proxy.ServeHTTP(c.Writer, c.Request)
	}
}
