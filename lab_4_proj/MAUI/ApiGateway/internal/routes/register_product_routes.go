package routes

import (
	"api-gateway/internal/services"

	"github.com/gin-gonic/gin"
)

func RegisterProductRoutes(router *gin.Engine, serviceURL string, mainApiRoute string) {
	router.POST(mainApiRoute+"/Products", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Products", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.DELETE(mainApiRoute+"/Products/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Products/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.PUT(mainApiRoute+"/Products/", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
}
