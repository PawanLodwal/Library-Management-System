using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LibraryManagementSystems
{
    public partial class Form1 : Form
    {
        List<Book> books = new List<Book>();


        public Form1()
        {
            InitializeComponent();
            LoadData(); // Load the data when the form loads
        }

        // Method to clear input fields after adding/updating book
        private void ClearInputFields()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtPublication.Clear();
            txtYear.Clear();
            txtSearch.Clear();
        }

        // Method to refresh the DataGridView with the list of books
        private void UpdateBookGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = books;
        }

        // Add Button Click Event
        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if any input fields are empty
                if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtAuthor.Text) ||
                    string.IsNullOrEmpty(txtPublication.Text) || string.IsNullOrEmpty(txtYear.Text))
                {
                    MessageBox.Show("Please fill all the required fields.");
                    return;
                }

                // Validate if Year field is numeric and in valid range
                if (!int.TryParse(txtYear.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
                {
                    MessageBox.Show("Please enter a valid year between 1900 and " + DateTime.Now.Year + ".");
                    return;
                }

                // Create a new book and add it to the list
                Book newBook = new Book(books.Count + 1, txtTitle.Text, txtAuthor.Text, txtPublication.Text, year);
                books.Add(newBook);

                MessageBox.Show("Book added successfully!");

                // Clear input fields and refresh DataGridView
                ClearInputFields();
                UpdateBookGrid();
                SaveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding book: " + ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a book is selected for update
                if (dataGridView1.SelectedRows.Count == 1)
                {
                    int selectedIndex = dataGridView1.SelectedRows[0].Index;
                    int bookId = Convert.ToInt32(dataGridView1[0, selectedIndex].Value);

                    Book selectedBook = books.Find(b => b.ID == bookId);

                    // Check if any input fields are empty
                    if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtAuthor.Text) ||
                        string.IsNullOrEmpty(txtPublication.Text) || string.IsNullOrEmpty(txtYear.Text))
                    {
                        MessageBox.Show("Please fill all the required fields.");
                        return;
                    }

                    // Validate if Year field is numeric and in valid range
                    if (!int.TryParse(txtYear.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
                    {
                        MessageBox.Show("Please enter a valid year between 1900 and " + DateTime.Now.Year + ".");
                        return;
                    }

                    // Update the selected book's details
                    selectedBook.Title = txtTitle.Text;
                    selectedBook.Author = txtAuthor.Text;
                    selectedBook.Publication = txtPublication.Text;
                    selectedBook.Year = year;

                    MessageBox.Show("Book updated successfully!");

                    // Clear input fields after update
                    ClearInputFields();

                    // Refresh DataGridView to show updated book list
                    UpdateBookGrid();
                    SaveData();
                }
                else
                {
                    MessageBox.Show("Please select a book to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating book: " + ex.Message);
            }
        }

        // Delete Button Click Event
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 1)
                {
                    int selectedIndex = dataGridView1.SelectedRows[0].Index;
                    int bookId = Convert.ToInt32(dataGridView1[0, selectedIndex].Value);

                    books.RemoveAll(b => b.ID == bookId);
                    MessageBox.Show("Book deleted successfully!");

                    // Refresh DataGridView to show updated book list
                    UpdateBookGrid();
                    SaveData();
                }
                else
                {
                    MessageBox.Show("Please select a book to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting book: " + ex.Message);
            }
        }

        // Search Functionality
        private void button4_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            List<Book> searchResults = books.FindAll(book =>
                book.Title.ToLower().Contains(searchTerm) ||
                book.Author.ToLower().Contains(searchTerm)
            );

            if (searchResults.Count > 0)
            {
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = searchResults;
            }
            else
            {
                MessageBox.Show("No matching books found.");
            }
        }

        // Issue Book
        private void btnIssueBook_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;
                Book selectedBook = books[selectedIndex];

                if (!selectedBook.IsIssued)
                {
                    selectedBook.IsIssued = true;
                    MessageBox.Show("Book issued successfully.");
                }
                else
                {
                    MessageBox.Show("Book is already issued.");
                }
                UpdateBookGrid();
                SaveData();
            }
            else
            {
                MessageBox.Show("Please select a book to issue.");
            }
        }
        // Method to save data
        public void SaveData()
        {
            try
            {
                using (FileStream fs = new FileStream("books.dat", FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    byte[] encryptedBooks = EncryptionHelper.EncryptData(books);  // Encrypt the book data
                    formatter.Serialize(fs, encryptedBooks);  // Serialize the encrypted data to the file
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message);
            }
        }

        // Load encrypted data from file
        public void LoadData()
        {
            string filePath = "books.dat";

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Check if the file is not empty
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 0)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            byte[] encryptedData = (byte[])formatter.Deserialize(fs);  // Deserialize the encrypted data
                            books = EncryptionHelper.DecryptData(encryptedData);  // Decrypt the data
                            UpdateBookGrid();  // Assuming this method updates the UI with the loaded data
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading data: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("The data file is empty.");
                }
            }
            else
            {
                MessageBox.Show("Data file not found.");
            }
        }

        // AES Encryption Helper class
        public static class EncryptionHelper
        {
            // AES Encryption method
            public static byte[] EncryptData(List<Book> books)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, books);  // Serialize the books list into a byte array
                    byte[] plainData = memoryStream.ToArray();  // Get the plain data (not encrypted)

                    using (Aes aes = Aes.Create())
                    {
                        // Ensure the key and IV are 16 bytes (128 bits) for AES-128
                        string keyString = "your-16-byte-key!";  // The key (ensure it's 16 bytes long)
                        string ivString = "your-16-byte-iv!!";   // The IV (ensure it's 16 bytes long)

                        // Pad or truncate the key and IV to ensure they are 16 bytes
                        byte[] key = Encoding.UTF8.GetBytes(keyString.PadRight(16, ' ').Substring(0, 16));  // Ensure 16 bytes
                        byte[] iv = Encoding.UTF8.GetBytes(ivString.PadRight(16, ' ').Substring(0, 16));   // Ensure 16 bytes

                        // Check if the key and IV are the correct size
                        if (key.Length != 16 || iv.Length != 16)
                        {
                            throw new InvalidOperationException("Key and IV must be 16 bytes.");
                        }

                        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                        using (MemoryStream encryptedStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainData, 0, plainData.Length);  // Write the data to the encryption stream
                            }

                            return encryptedStream.ToArray();  // Return the encrypted data
                        }
                    }
                }
            }

            // AES Decryption method
            public static List<Book> DecryptData(byte[] encryptedData)
            {
                using (Aes aes = Aes.Create())
                {
                    string keyString = "your-16-byte-key!";  // The key (ensure it's 16 bytes long)
                    string ivString = "your-16-byte-iv!!";   // The IV (ensure it's 16 bytes long)

                    // Pad or truncate the key and IV to ensure they are 16 bytes
                    byte[] key = Encoding.UTF8.GetBytes(keyString.PadRight(16, ' ').Substring(0, 16));  // Ensure 16 bytes
                    byte[] iv = Encoding.UTF8.GetBytes(ivString.PadRight(16, ' ').Substring(0, 16));   // Ensure 16 bytes

                    // Check if the key and IV are the correct size
                    if (key.Length != 16 || iv.Length != 16)
                    {
                        throw new InvalidOperationException("Key and IV must be 16 bytes.");
                    }

                    ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

                    using (MemoryStream encryptedStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (MemoryStream decryptedStream = new MemoryStream())
                            {
                                cryptoStream.CopyTo(decryptedStream);  // Decrypt the stream to memory
                                byte[] decryptedData = decryptedStream.ToArray();

                                // Deserialize the decrypted data into a List of Book objects
                                BinaryFormatter binaryFormatter = new BinaryFormatter();
                                using (MemoryStream ms = new MemoryStream(decryptedData))
                                {
                                    return (List<Book>)binaryFormatter.Deserialize(ms);  // Return the deserialized list of books
                                }
                            }
                        }
                    }
                }
            }
        }
        

    }
}


