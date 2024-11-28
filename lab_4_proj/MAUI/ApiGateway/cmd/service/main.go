package main

import (
	"api-gateway/internal/config"
	"api-gateway/internal/middleware"
	"api-gateway/internal/routes"

	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
)

func main() {
	cfg := config.NewConfig("../../.env")

	r := gin.Default()

	serviceURLs := []string{
		cfg.ProductServiceURL + "/swagger/v1/swagger.json",
	}

	r.Use(cors.New(cors.Config{
		AllowAllOrigins: true,
		AllowMethods:    []string{"GET", "POST", "PUT", "DELETE", "OPTIONS"},
		AllowHeaders:    []string{"Origin", "Content-Type", "Accept", "ngrok-skip-browser-warning"},
	}))
	r.Use(middleware.RequestLogger())
	r.Use(middleware.RecoveryMiddleware())
	r.Use(middleware.RateLimiter(cfg.RequestSentLimit, cfg.RequestSentTimeWindow))
	r.Use(middleware.RequestSizeLimiter(cfg.RequestSizeLimit * 1024 * 1024)) // x * 1MB

	routes.RegisterProductRoutes(r, cfg.ProductServiceURL, cfg.MainApiRoute)
	routes.RegisterSwaggerRoutes(r, serviceURLs, cfg.MainApiRoute)

	r.Run(cfg.ServicePort)
}
