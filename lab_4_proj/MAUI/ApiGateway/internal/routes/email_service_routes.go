package routes

import (
	"api-gateway/internal/services"

	"github.com/gin-gonic/gin"
)

func RegisterEmailRoutes(router *gin.Engine, serviceURL string, mainApiRoute string) {
	router.GET(mainApiRoute+"/emails/:id", services.ReverseProxy(serviceURL, "/email-service/api", mainApiRoute))
	router.GET(mainApiRoute+"/emails", services.ReverseProxy(serviceURL, "/email-service/api", mainApiRoute))
	router.POST(mainApiRoute+"/send-email", services.ReverseProxy(serviceURL, "/email-service/api", mainApiRoute))
}
