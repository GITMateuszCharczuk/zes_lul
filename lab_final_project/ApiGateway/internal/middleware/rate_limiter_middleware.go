package middleware

import (
	"net/http"
	"sync"
	"time"

	"github.com/gin-gonic/gin"
)

type rateLimiter struct {
	visits map[string]int
	mu     sync.Mutex
}

var limiter = &rateLimiter{
	visits: make(map[string]int),
}

func RateLimiter(limit int, window time.Duration) gin.HandlerFunc {
	return func(c *gin.Context) {
		clientIP := c.ClientIP()

		limiter.mu.Lock()
		defer limiter.mu.Unlock()

		limiter.visits[clientIP]++

		if limiter.visits[clientIP] > limit {
			c.AbortWithStatusJSON(http.StatusTooManyRequests, gin.H{"error": "Rate limit exceeded"})
			return
		}

		go func() {
			time.Sleep(window)
			limiter.mu.Lock()
			defer limiter.mu.Unlock()
			limiter.visits[clientIP] = 0
		}()

		c.Next()
	}
}
