import React, { useEffect, useState } from 'react';
import API from '../services/API1'; // Axios instance with token header setup

const Dashboard = () => {
  const [books, setBooks] = useState([]); // All books created by this user
  const [formData, setFormData] = useState({ name: '', category: '', price: '', description: '' }); // Form for new book
  const [editingBook, setEditingBook] = useState(null); // Book being edited, if any

  // Start editing a selected book
  const startEdit = (book) => {
    setEditingBook({ ...book });
  };

  // Update editingBook state when input changes
  const handleEditChange = (e) => {
    const { name, value } = e.target;
    setEditingBook({ ...editingBook, [name]: value });
  };

  // Save changes to a book
  const handleSave = async (id) => {
    try {
      await API.put(`/book/${id}`, {
        ...editingBook,
        price: parseFloat(editingBook.price) // Ensure price is a number
      });
      setEditingBook(null); // Clear edit mode
      fetchBooks(); // Refresh book list
    } catch (error) {
      console.error(error);
      alert('Update failed.');
    }
  };

  // Cancel editing
  const cancelEdit = () => {
    setEditingBook(null);
  };

  // Delete a book after user confirms
  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this book?")) return;

    try {
      await API.delete(`/book/${id}`);
      fetchBooks(); // Refresh after delete
    } catch (error) {
      console.error(error);
      alert('Delete failed.');
    }
  };

  // Fetch all books created by the logged-in user
    const fetchBooks = async () => {
    try {
      const response = await API.get('/book');
      console.log("Fetched books:", response.data);
      setBooks(response.data);
    } catch (error) {
      console.error("Error fetching books:", error);
    }
  };

  // Handle form submission for adding a new book
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await API.post('/book', {
        ...formData,
        price: parseFloat(formData.price)
      });
      setFormData({ name: '', category: '', price: '', description: '' }); // Clear form
      fetchBooks();
    } catch (error) {
      console.error(error);
      alert('Failed to add book. Make sure you are logged in.');
    }
  };

  // Update formData as user types
  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  // Load books on component mount
  useEffect(() => {
  const token = localStorage.getItem('token');
  if (token) {
    fetchBooks(); // ✅ Only fetch if token is present
  } else {
    console.warn("No token found!");
  }
  }, []);


  // Logout by removing token and redirecting to login
  const handleLogout = () => {
    localStorage.removeItem('token');
    window.location.href = '/';
  };

  return (
    <div className="container mt-4">
      {/* Header with logout */}
      <div className="d-flex justify-content-between align-items-center">
        <h2>📚 Book Dashboard</h2>
        <button className="btn btn-outline-danger btn-sm" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {/* Book submission form */}
      <form onSubmit={handleSubmit} className="row g-3 mt-4 mb-5">
        <div className="col-md-3">
          <input type="text" name="name" placeholder="Book Name" className="form-control" value={formData.name} onChange={handleChange} required />
        </div>
        <div className="col-md-2">
          <input type="text" name="category" placeholder="Category" className="form-control" value={formData.category} onChange={handleChange} required />
        </div>
        <div className="col-md-2">
          <input type="number" name="price" placeholder="Price" className="form-control" value={formData.price} onChange={handleChange} required />
        </div>
        <div className="col-md-4">
          <input type="text" name="description" placeholder="Description" className="form-control" value={formData.description} onChange={handleChange} required />
        </div>
        <div className="col-md-1 d-grid">
          <button type="submit" className="btn btn-success">Add</button>
        </div>
      </form>

      {/* Book list table */}
      <table className="table table-bordered">
        <thead className="table-light">
          <tr>
            <th>ID</th><th>Name</th><th>Category</th><th>Price</th><th>Description</th>
          </tr>
        </thead>
        <tbody>
          {books.map((book) => (
            <tr key={book.id}>
              <td>{book.id}</td>
              {/* Editable fields */}
              <td>{editingBook?.id === book.id ? (
                <input type="text" name="name" className="form-control" value={editingBook.name} onChange={handleEditChange} />
              ) : book.name}</td>

              <td>{editingBook?.id === book.id ? (
                <input type="text" name="category" className="form-control" value={editingBook.category} onChange={handleEditChange} />
              ) : book.category}</td>

              <td>{editingBook?.id === book.id ? (
                <input type="number" name="price" className="form-control" value={editingBook.price} onChange={handleEditChange} />
              ) : `$${book.price.toFixed(2)}`}</td>

              <td>{editingBook?.id === book.id ? (
                <input type="text" name="description" className="form-control" value={editingBook.description} onChange={handleEditChange} />
              ) : book.description}</td>

              {/* Action buttons */}
              <td className="text-nowrap">
                {editingBook?.id === book.id ? (
                  <>
                    <button className="btn btn-sm btn-success me-2" onClick={() => handleSave(book.id)}>Save</button>
                    <button className="btn btn-sm btn-secondary" onClick={cancelEdit}>Cancel</button>
                  </>
                ) : (
                  <>
                    <button className="btn btn-sm btn-warning me-2" onClick={() => startEdit(book)}>Edit</button>
                    <button className="btn btn-sm btn-danger" onClick={() => handleDelete(book.id)}>Delete</button>
                  </>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Dashboard;
