package routes

import (
	"api-gateway/internal/services"

	"github.com/gin-gonic/gin"
)

func RegisterProductRoutes(router *gin.Engine, serviceURL string, mainApiRoute string) {
	// Product routes
	router.POST(mainApiRoute+"/Products", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Products", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.DELETE(mainApiRoute+"/Products/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Products/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.PUT(mainApiRoute+"/Products/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))

	// Category routes
	router.GET(mainApiRoute+"/Categories", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Categories/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.DELETE(mainApiRoute+"/Categories/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))

	// Auth routes
	router.POST(mainApiRoute+"/Auth/login", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.POST(mainApiRoute+"/Auth/register", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.POST(mainApiRoute+"/Auth/promote/:userId", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.POST(mainApiRoute+"/Auth/demote/:userId", services.ReverseProxy(serviceURL, "/api", mainApiRoute))

	// Order routes
	router.GET(mainApiRoute+"/Orders", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Orders/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Orders/user/:userId", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.POST(mainApiRoute+"/Orders", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.PUT(mainApiRoute+"/Orders/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.PUT(mainApiRoute+"/Orders/:id/status", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.DELETE(mainApiRoute+"/Orders/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))

	router.POST(mainApiRoute+"/Users", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Users", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.DELETE(mainApiRoute+"/Users/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.GET(mainApiRoute+"/Users/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
	router.PUT(mainApiRoute+"/Users/:id", services.ReverseProxy(serviceURL, "/api", mainApiRoute))
}
