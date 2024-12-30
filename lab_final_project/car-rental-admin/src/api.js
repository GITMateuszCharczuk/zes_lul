import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:8800/whatever/api', // Replace with your API base URL
  headers: {
    "ngrok-skip-browser-warning":"any"
  },
});

export default api;
