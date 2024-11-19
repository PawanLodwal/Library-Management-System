
using System;

[Serializable]
public class Book
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Publication { get; set; }
    public int Year { get; set; }
    public bool IsIssued { get; set; }

    public Book(int id, string title, string author, string publication, int year)
    {
        ID = id;
        Title = title;
        Author = author;
        Publication = publication;
        Year = year;
        IsIssued = false;
    }
}