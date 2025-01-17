package middleware

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

func RequestSizeLimiter(maxBytes int64) gin.HandlerFunc {
	return func(c *gin.Context) {
		c.Request.Body = http.MaxBytesReader(c.Writer, c.Request.Body, maxBytes)
		if err := c.Request.ParseForm(); err != nil {
			c.AbortWithStatusJSON(http.StatusRequestEntityTooLarge, gin.H{"error": "Request size exceeds limit"})
			return
		}
		c.Next()
	}
}
