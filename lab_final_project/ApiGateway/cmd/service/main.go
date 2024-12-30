package main

import (
	"api-gateway/internal/config"
	"api-gateway/internal/middleware"
	"api-gateway/internal/routes"
	"net/http"

	"github.com/gin-gonic/gin"
)

func main() {
	cfg := config.NewConfig("../../.env")

	r := gin.Default()

	serviceURLs := []string{
		cfg.ProductServiceURL + "/swagger/v1/swagger.json",
	}
	r.Use(middleware.CorsMiddleware())
	r.Use(middleware.RequestLogger())
	r.Use(middleware.RecoveryMiddleware())
	r.Use(middleware.RateLimiter(cfg.RequestSentLimit, cfg.RequestSentTimeWindow))
	r.Use(middleware.RequestSizeLimiter(cfg.RequestSizeLimit * 1024 * 1024)) // x * 1MB

	r.OPTIONS("/*path", func(c *gin.Context) {
		c.Status(http.StatusOK)
	})
	routes.RegisterProductRoutes(r, cfg.ProductServiceURL, cfg.MainApiRoute)
	routes.RegisterSwaggerRoutes(r, serviceURLs, cfg.MainApiRoute)

	r.Run(cfg.ServicePort)
}
