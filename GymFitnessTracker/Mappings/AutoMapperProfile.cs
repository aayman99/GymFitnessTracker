using AutoMapper;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;

namespace GymFitnessTracker.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<Exercise, ExerciseDto>().ReverseMap();
            CreateMap<Exercise, ExerciseDto>()
                .ForMember(dest => dest.PrimaryMuscle, opt => opt.MapFrom(src => src.PrimaryMuscle.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Title)).ReverseMap();
            CreateMap<Exercise, FemaleExerciseDto>()
                .ForMember(dest => dest.PrimaryMuscle, opt => opt.MapFrom(src => src.PrimaryMuscle.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Title)).ReverseMap();
            CreateMap<Exercise, MaleExerciseDto>()
                .ForMember(dest => dest.PrimaryMuscle, opt => opt.MapFrom(src => src.PrimaryMuscle.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Title)).ReverseMap();
            CreateMap<Exercise, AddExerciseRequestDto>().ReverseMap();
            CreateMap<Exercise, UpdateExerciseRequestDto>().ReverseMap();
            CreateMap<Workout, WorkoutDto>().ReverseMap();
            CreateMap<WorkoutExercise, WorkoutExerciseDto>().ReverseMap();
            CreateMap<Workout,AddWorkoutRequestDto>().ReverseMap();
            /*CreateMap<WorkoutExercise, WorkoutExerciseDto>()
                .ForMember(dest => dest.ExerciseTitle, opt => opt.MapFrom(src => src.Exercise.Title))
                .ReverseMap();*/
            CreateMap<WorkoutExercise, WorkoutExerciseDto>()
                .ForMember(dest => dest.ExerciseTitle, opt => opt.MapFrom
                     (src => 
                        src.Exercise != null ? src.Exercise.Title :
                        src.CustomExercise != null ? src.CustomExercise.Title :
                        ""
                     )
                )
                .ReverseMap();
            //CreateMap<Set, SetDto>().ReverseMap();
            CreateMap<Set, SetDto>()
                .ForMember(dest => dest.TimeUnit, opt => opt.MapFrom
                    (src => src.TimeUnit != null ? src.TimeUnit.Title : ""))
                .ForMember(dest => dest.WeightUnit, opt => opt.MapFrom
                    (src => src.WeightUnit != null ? src.WeightUnit.Title : ""));
            CreateMap<UpdateSetRequestDto, SetDto>().ReverseMap();
            CreateMap<Plan, PlanDto>().ReverseMap();
            CreateMap<CustomExercise, CustomExerciseDto>()
                /*.ForMember(dest => dest.PrimaryMuscle, opt => opt.MapFrom(src => src.PrimaryMuscle.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Title))*/
                .ReverseMap();

            CreateMap<CustomExercise, AddCustomExerciseRequestDto>().ReverseMap();
            CreateMap<CustomExercise, UpdateCustomExerciseRequestDto>().ReverseMap();

        }
    }
}
