using LibraryData;
using LibraryData.Models;
using LibraryServices.BaseService;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : BaseService<LibraryAsset>, ILibraryAssetService
    {
        #region Constructor

        public LibraryAssetService(LibraryContext context) : base(context)
        {
        }

        #endregion Constructor

        #region ILibraryAsset Implementation
        public override IEnumerable<LibraryAsset> GetAll()
        {
            return libraryContext.LibraryAssets
                        .Include(asset => asset.Status)
                        .Include(asset => asset.Location);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (libraryContext.Books.Any(book => book.Id == id))
            {
                return libraryContext.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }

            return "";
        }

        public string GetIsbn(int id)
        {
            if (libraryContext.Books.Any(book => book.Id == id))
            {
                return libraryContext.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }

            return "";
        }

        public string GetTitle(int id)
        {
            return GetById(id).Title;
        }

        public string GetType(int id)
        {
            var book = libraryContext.LibraryAssets.OfType<Book>()
                            .Where(asset => asset.Id == id);
            return book.Any() ? "Book" : "Video";
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = libraryContext.LibraryAssets.OfType<Book>()
                            .Where(asset => asset.Id == id).Any();

            var isVideo = libraryContext.LibraryAssets.OfType<Video>()
                            .Where(asset => asset.Id == id).Any();

            return isBook ? libraryContext.Books.FirstOrDefault(book => book.Id == id).Author :
                            libraryContext.Videos.FirstOrDefault(video => video.Id == id).Director
                            ?? "Unknown";
        }

        #endregion ILibraryAsset Implementation
    }
}