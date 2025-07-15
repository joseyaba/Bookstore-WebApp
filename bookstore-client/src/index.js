import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css'; // Global CSS if any
import App from './App'; // Main App component
import reportWebVitals from './reportWebVitals';
import 'bootstrap/dist/css/bootstrap.min.css'; // Load Bootstrap CSS for styling

// Mount the React app into the root <div id="root"> in index.html
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App /> {/* Render the main App component */}
  </React.StrictMode>
);

// Optional: Web Vitals reporting (performance metrics)
reportWebVitals();
