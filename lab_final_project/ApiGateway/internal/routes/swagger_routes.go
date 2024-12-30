package routes

import (
	"api-gateway/internal/services"
	"net/http"

	"github.com/gin-gonic/gin"
)

func RegisterSwaggerRoutes(router *gin.Engine, serviceURLs []string, mainApiRoute string) {
	router.Static("/swagger-ui", "./swagger-ui")

	router.GET(mainApiRoute+"/swagger/doc.json", func(c *gin.Context) {
		combinedSwagger := services.AggregateSwagger(serviceURLs, mainApiRoute)

		c.JSON(200, combinedSwagger)
	})

	router.LoadHTMLFiles("swagger-ui/dynamic_index.html")

	router.GET(mainApiRoute+"/swagger/index.html", func(c *gin.Context) {
		c.HTML(http.StatusOK, "dynamic_index.html", nil)
	})
}
