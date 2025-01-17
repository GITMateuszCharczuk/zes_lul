package routes

import (
	"api-gateway/internal/services"

	"github.com/gin-gonic/gin"
)

func RegisterFileRoutes(router *gin.Engine, serviceURL string, mainApiRoute string) {
	router.POST(mainApiRoute+"/files", services.ReverseProxy(serviceURL, "/file-storage/api", mainApiRoute))
	router.GET(mainApiRoute+"/files/get", services.ReverseProxy(serviceURL, "/file-storage/api", mainApiRoute))
	router.DELETE(mainApiRoute+"/files/delete", services.ReverseProxy(serviceURL, "/file-storage/api", mainApiRoute))
}
