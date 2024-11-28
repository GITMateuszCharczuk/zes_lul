import axios from 'axios';

const api = axios.create({
  baseURL: 'https://88d0-2a02-a319-ee-a480-f324-8852-24d6-318f.ngrok-free.app/whatever/api', // Replace with your API base URL
  headers: {
    "ngrok-skip-browser-warning":"any"
  },
});

export default api;
