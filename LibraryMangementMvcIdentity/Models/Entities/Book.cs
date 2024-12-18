using System.ComponentModel.DataAnnotations;

namespace LibraryMangementMvcIdentity.Models.Entities
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }



        /*
         NOTICE: that it is not in the table I added later to handle race condition for sending request this method is called : Optimistic Concurrency Control
         for now I am using Database Locking wich uses _context.Database.BeginTransactionAsync() => check UserSevice class => AddRequestAsync()

         The RowVersion column is automatically checked by EF during SaveChangesAsync().
         If another request modifies the row between when it is read and when changes are saved, a DbUpdateConcurrencyException is thrown.
         */
        //[Timestamp]
        //public byte[] RowVersion { get; set; } // Concurrency token
    }
}
