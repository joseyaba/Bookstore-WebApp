// src/services/API1.js
import axios from 'axios';

const API = axios.create({
  baseURL: 'https://localhost:7045/api',
});

// Attach token to every request
API.interceptors.request.use((config) => {
  const token = localStorage.getItem('token'); // ⬅️ Get token fresh each time
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

export default API;
