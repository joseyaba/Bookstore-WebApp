import React from 'react';
import { Navigate } from 'react-router-dom';

// This component wraps protected routes.
// If the user has a token in localStorage, it allows access.
// Otherwise, it redirects the user to the login page.
const ProtectedRoute = ({ children }) => {
  const token = localStorage.getItem('token'); // Get JWT token from localStorage
  return token ? children : <Navigate to="/" />; // If token exists, show children; else redirect
};

export default ProtectedRoute;
