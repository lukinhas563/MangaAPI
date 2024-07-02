using AutoMapper;
using MangaAPI.Entities;
using MangaAPI.Models.Input;
using MangaAPI.Models.View;

namespace MangaAPI.Mappers;

public class MangaProfiles: Profile
{
    public MangaProfiles()
    {
        // Manga
        CreateMap<Mangas, MangaListViewModel>();
        CreateMap<Mangas, MangaViewModel>()
                .ForMember(dest => dest.Chapters, opt => opt.Ignore());

        // Chapters
        CreateMap<Chapter, ChapterListViewModel>();
        CreateMap<Chapter, ChapterViewModel>();

        CreateMap<ChapterInputModel, Chapter>();

        // Pages
        CreateMap<PageManga, PageViewModel>();

        // Users
        CreateMap<User, UserViewModel>();

        CreateMap<UserInputModel, User>();
    }
}
