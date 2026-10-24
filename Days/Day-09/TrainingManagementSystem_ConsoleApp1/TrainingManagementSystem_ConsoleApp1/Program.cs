
using TrainingManagementSystem_ConsoleApp1.Exception;
using TrainingManagementSystem_ConsoleApp1.Services;

TrainingService trainingService = new TrainingService();
LearnerService learnerService = new LearnerService();

trainingService.DisplayCourseDetails();
try
{
    trainingService.SearchCourseByCode("C005", "Software Engineering Principles");
}catch(CourseNotFoundException ex)
{
    Console.WriteLine(ex.Message);
}
trainingService.SortCoursesByFee(true);
trainingService.AvailableSeats();

learnerService.RegisterLearner();
learnerService.RegisterLearner();
learnerService.DisplayLearners();

EnrollmentService enrollmentService =
    new EnrollmentService(trainingService, learnerService);

enrollmentService.EnrollLearner();
enrollmentService.DisplayEnrollments();