import React, { useState } from 'react';
import API from '../services/API1'; // Custom Axios instance
import { useNavigate } from 'react-router-dom';

const LoginPage = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  // Handles login form submission
  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await API.post('/auth/login', { username, password });
      const token = response.data.token;

      console.log("Token received:", token);
      localStorage.setItem('token', token); // Save token to localStorage

      navigate('/dashboard'); // ✅ Use React Router navigation
    } catch (error) {
      alert('Login failed!');
    }
  };

  return (
    <div className="container mt-5">
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <input
          className="form-control mb-2"
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
        <input
          className="form-control mb-2"
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <button className="btn btn-primary" type="submit">Login</button>
      </form>
    </div>
  );
};

export default LoginPage;
