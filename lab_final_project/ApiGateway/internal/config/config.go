package config

import (
	"fmt"
	"os"
	"strconv"
	"strings"
	"sync"
	"time"

	"github.com/joho/godotenv"
)

type Config struct {
	ServicePort           string
	Env                   string
	RequestSentLimit      int
	RequestSentTimeWindow time.Duration
	RequestSizeLimit      int64
	MainApiRoute          string
	ProductServiceURL     string
}

var (
	instance *Config
	once     sync.Once
)

func NewConfig(path string) *Config {
	var err error
	once.Do(func() {
		env := getEnv("ENV", "")
		if strings.ToLower(env) == "test" || env == "" {
			err = godotenv.Load(path)
		}

		if err != nil && strings.ToLower(env) != "prod" {
			panic(err)
		}
		RequestSentLimitEnv := getEnv("REQUEST_SENT_LIMIT", "100")
		RequestSentLimit, err := strconv.Atoi(RequestSentLimitEnv)
		if err != nil {
			fmt.Println("Failed to convert RequestSentLimit env to int:", err)
			return
		}
		RequestSizeLimitEnv := getEnv("REQUEST_SENT_TIME_WINDOW", "60")
		RequestSizeLimit, err := strconv.ParseInt(RequestSizeLimitEnv, 10, 64)
		if err != nil {
			fmt.Println("Failed to convert RequestSizeLimit env to int:", err)
			return
		}
		RequestSentTimeWindowEnv := getEnv("REQUEST_SIZE_LIMIT", "10")
		RequestSentTimeWindowSeconds, err := strconv.Atoi(RequestSentTimeWindowEnv)
		if err != nil {
			fmt.Println("Failed to convert RequestSentTimeWindow env to time second:", err)
			return
		}
		RequestSentTimeWindowMinutes := time.Duration(RequestSentTimeWindowSeconds) * time.Second

		instance = &Config{
			ServicePort:           getEnv("SERVICE_PORT", "8080"),
			Env:                   getEnv("ENV", "test"),
			RequestSentLimit:      RequestSentLimit,
			RequestSentTimeWindow: RequestSentTimeWindowMinutes,
			RequestSizeLimit:      RequestSizeLimit,
			MainApiRoute:          getEnv("MAIN_API_ROUTE", "/car-rental/api"),
			ProductServiceURL:     getEnv("PRODUCT_SERVICE_URL", "http://product-service:8080"),
		}
	})
	return instance
}

func GetConfig() *Config {
	return instance
}

func getEnv(key, fallback string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return fallback
}
