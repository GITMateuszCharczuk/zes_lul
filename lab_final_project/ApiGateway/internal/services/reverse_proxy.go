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
		remote, err := url.Parse(serviceURL)
		if err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": "Service unavailable"})
			return
		}

		proxy := httputil.NewSingleHostReverseProxy(remote)
		proxy.Director = func(req *http.Request) {
			req.URL.Scheme = remote.Scheme
			req.URL.Host = remote.Host

			// Replace the main API route with the service path
			if strings.HasPrefix(req.URL.Path, mainApiRoute) {
				req.URL.Path = strings.Replace(req.URL.Path, mainApiRoute, servicePath, 1)
			}

			// Ensure host header is set correctly
			req.Host = remote.Host
		}

		proxy.ModifyResponse = func(resp *http.Response) error {
			// Add debug headers if needed
			resp.Header.Add("X-Proxy-Debug", "true")
			return nil
		}

		proxy.ErrorHandler = func(rw http.ResponseWriter, req *http.Request, err error) {
			c.JSON(http.StatusBadGateway, gin.H{
				"error":   "Service temporarily unavailable",
				"details": err.Error(),
			})
		}

		proxy.ServeHTTP(c.Writer, c.Request)
	}
}
