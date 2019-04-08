using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.DomainLogic
{
    public class BusinessLogicDataSeeder
    {
        private readonly BusinessLogic _bl;
        private readonly IRepository _repository;
        private readonly static object lockObject = new object();
        private static bool isSeeded;


        public BusinessLogicDataSeeder(BusinessLogic domainLogic, IRepository repository)
        {
            _bl = domainLogic;
            _repository = repository;
        }

        public void Seed()
        {
            System.Diagnostics.Debug.WriteLine($"!!!!!!!!!!!! - Seed()");
            if (!isSeeded)
            {
                lock (lockObject)
                {
                    if (isSeeded)
                    {
                        return;
                    }

                    if (!_bl.IsDbAccesible())
                    {
                        throw new ApplicationException("Unable to connect to db, db is not available");
                    }

                    System.Diagnostics.Debug.WriteLine($"!!!!!!!!!!!! - Seed() -Task.Run(async () => await SeedUsersAsync())");

                    var task = Task.Run(async () => await SeedUsersAsync());
                    task.Wait();

                    System.Diagnostics.Debug.WriteLine($"!!!!!!!!!!!! - Seed() - Task.Run(async () => await SeedQuestionnaireTemplatesAsync())");
                    task = Task.Run(async () => await SeedQuestionnaireTemplatesAsync());
                    task.Wait();

                    System.Diagnostics.Debug.WriteLine($"!!!!!!!!!!!! - Seed() - Task.Run(async () => await SeedBusinessLogicSettingsAsync())");
                    task = Task.Run(async () => await SeedBusinessLogicSettingsAsync());
                    task.Wait();

                    isSeeded = true;
                }
            }
            System.Diagnostics.Debug.WriteLine($"########### - Seed()");
        }

        private async Task SeedQuestionnaireTemplatesAsync()
        {
            var existingOnboardingQuestionnaire = await _bl.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, null);
            if (existingOnboardingQuestionnaire == null)
            {
                var newOnBoardingQuestionnaire = GetSampleData_Week_One_v180616();
                var newId = await _bl.CreateQuestionnaireTemplateAsync(newOnBoardingQuestionnaire);
            }

            return;
        }

        private async Task SeedBusinessLogicSettingsAsync()
        {
            var existingSettings = await _bl.GetBusinessLogicSettings();

            if (existingSettings == null || existingSettings.Version < BL_Settings.CurrentVersion)
            {
                var newId = await _bl.SetSettings(BL_Settings.GetDefaultValues());
            }

            return;
        }

        private static QuestionnaireTemplate GetSampleData_Week_One_v180616()
        {
            var text = "Move the slider to indicate your satisfaction level";

            var result = new QuestionnaireTemplate();
            result.Id = null;// "E069EF39-571C-4F65-A2FE-43A05B0BAB00";
            result.QuestionnaireSets = new List<QuestionnaireSet>();
            result.TypeOfQuestionnaire = QuestionnaireTemplate.QuestionnaireType.OnBoarding;
            result.TypeOfGender = QuestionnaireTemplate.GenderType.Any;
            result.IsActive = true;

            var questionsFor1stSet = new List<Question>();

            result.QuestionnaireSets.Add(new QuestionnaireSet
            {
                Id = null,// "E069EF39-571C-4F65-A2FE-43A05B0BAB01",
                NextButtonText = "BEGIN QUESTIONNAIRE",
                Text = "This is an important step used to understand your relationship dynamics and to help craft your custom experience in LifeCouple.  The questionnaire will only take a few minutes.",
                Title = "Onboarding questions",
                Questions = questionsFor1stSet
            });

            #region 1stSet
            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a01",
                Title = "Affection",
                Description = @"Affection takes the loving relationship between a man and woman into the deeper realm of tender expressions. LifeCouple views affection as giving words of affirmation, acts of service, gifts, and physical touch.

Using this or your definition of affection.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall }
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a02",
                Title = "Intimacy",
                Description = @"LifeCouple’s view is an interpersonal relationship that involves physical plus emotional intimacy. Physical intimacy is characterized by friendship, platonic love, romantic love, and enjoyable sexual activity.

Using this or your definition of a sexual relationship.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 3, TypeOfIndex = IndexTypeEnum.Overall },
                    new IndexImpact { CalculationWeight = 10, TypeOfIndex = IndexTypeEnum.Intimacy }
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a03",
                Title = "Emotional Connection",
                Description = @"LifeCouple’s view is a secure attachment, 'feeling in touch', 'can I get your attention when I need it', 'can you comfort me when needed', 'do you care about my well-being even when I am not around.' 

Using this or your definition of emotional connection.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a04",
                Title = "Empathy",
                Description = @"LifeCouple views empathy as putting yourself in the shoes of another to understand, be sensitive and share the feelings of another. 

Using this or your definition of empathy.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a05",
                Title = "Knowing and Understanding your Partner",
                Description = @"LifeCouple views knowing your partner as learning, showing interest, taking effort on important aspects of the relationship.  Understanding is truly listening and ultimately understanding on another’s needs and what matters to each of you. 

Using this or your definition of knowing and understanding your partner.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a06",
                Title = "Admiration",
                Description = @"Lifecouple views admiration as feeling respect, esteem and approval of one’s partner. 

Using this or your definition of admiration.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a07",
                Title = "Appreciation",
                Description = @"Lifecouple views appreciation as thankful recognition and gratitude towards one’s partner. 

Using this or your definition of appreciation.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 1, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a08",
                Title = "Daily Chores",
                Description = @"LifeCouple views daily chores as the management of daily, weekly and monthly duties involved in the running of a household, such as cleaning, cooking, home maintenance, shopping, and doing laundry. 

Using this or your definition of daily chores.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 1, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a09",
                Title = "Responsibilities",
                Description = @"LifeCouple views responsibilities as larger scale aspects such as dividing how to meet all of the children’s needs, planning for major events, managing finances, etc.  

Using this or your definition of responsibilities.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 1, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a10",
                Title = "Communication",
                Description = @"LifeCouple views communication as how you explain to your partner what you are experiencing and what your needs are and then having your partner affirm what you communicated. 

Using this or your definition of communication.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 3, TypeOfIndex = IndexTypeEnum.Overall },
                    new IndexImpact { CalculationWeight = 10, TypeOfIndex = IndexTypeEnum.Communication },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a11",
                Title = "Commitment",
                Description = @"LifeCouple views commitment as the effort to another involving love, trust, honesty, openness, other aspects that are important to both of you that includes willingness to do whatever it takes to make the relationship work. 

Using this or your definition of commitment.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a12",
                Title = "Trust",
                Description = @"LifeCouple views trust as a decision to love, be faithful, be reliable, and make each other feel safe, both emotionally and physically; and ultimately to make your relationship a top priority. 

Using this or your definition of trust.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 3, TypeOfIndex = IndexTypeEnum.Overall },
                    new IndexImpact { CalculationWeight = 10, TypeOfIndex = IndexTypeEnum.Trust },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a13",
                Title = "Healthy Conflict ",
                Description = @"LifeCouple’s view is that when disagreements arise in a relationship, healthy conflict includes deciding to handle the conflict in cooperative manner even if both sides do not agree and not resorting to attacking, fleeing or withdrawing. 

Using this or your definition of healthy conflict.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 3, TypeOfIndex = IndexTypeEnum.Overall },
                    new IndexImpact { CalculationWeight = 10, TypeOfIndex = IndexTypeEnum.ConflictResolution },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a14",
                Title = "Quality \"Couple Time\"",
                Description = @"LifeCouple’s view is sharing quality time together that includes a shared activity and interacting together as a couple without kids or other interruptions that is meaningful for the relationship. 

Using this or your definition of couple time.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a15",
                Title = "Support",
                Description = @"LifeCouple’s view includes supporting each other’s careers and goals as well as supporting each other in rough times. 

Using this or your definition of support.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a16",
                Title = "Respect",
                Description = @"LifeCouple’s view includes a positive feeling and action in a way that shows that you are aware of your partner’s needs and feelings and honoring your partner by exhibiting care, concern and consideration.

Using this or your definition of respect.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 2, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new Question
            {
                Id = null,//"a17",
                Title = "Decisions",
                Description = @"LifeCouple’s view is that relationships have important decisions that are made over time including where to live, having children, working/staying at home with children, parenting styles. 

Using this or your definition of decisions.",
                Text = text,
                TypeOfQuestion = Question.QuestionType.Range,
                IndexesImpacted = new List<IndexImpact> {
                    new IndexImpact { CalculationWeight = 1, TypeOfIndex = IndexTypeEnum.Overall },
                },
                AnswerOptions = null,
            });

            foreach (var item in questionsFor1stSet)
            {
                if (item.TypeOfQuestion == Question.QuestionType.Range)
                {
                    item.MaxRange = item.MaxRange ?? 10;
                    item.MinRange = item.MinRange ?? 0;
                }
            }

            #endregion



            return result;
        }

        private async Task SeedUsersAsync()
        {
            if (!_repository.IsDbAccessible())
            {
                throw new ApplicationException("Unable to connect to db, db is not available");
            }

            var existingUsers = await _repository.FindUserProfiles_byPrimaryEmailAsync("pgrimskog@lifecouple.net");

            if (existingUsers.Count() != 0)
            {
                System.Diagnostics.Debug.WriteLine("SeedUsersAsync() - not adding users, already exists");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SeedUsersAsync() - adding users");
                var newUser = await _repository.CreateUserAsync("pgrimskog@lifecouple.net", "EA5CEE24-90F9-4804-A45A-4D2E7FCA5D1A", true, "Per-lc", "Grimskog-lc", null);
                newUser.Gender = "m";
                await _repository.SetUserProfileAsync(newUser);
                await _bl.SetRelationshipAsync(new BL_Relationship //Added this relationship 180903
                {
                    BeenToCounseler = BL_Relationship.CounselerStatusEnum.NoNever,
                    CurrentWeddingDate = DateTime.Now.AddYears(-10),
                    MarriageStatus = BL_Relationship.MarriageStatusEnum.FirstMarriage,
                    NrOfChildren = 0,
                    NrOfStepChildren = 0,
                    RegisteredPartner_Id = newUser.Id,
                    RelationshipStatus = BL_Relationship.RelationshipStatusEnum.Married,
                }, newUser.Id);
                var up = await _repository.GetUserProfile_byIdAsync(newUser.Id);

                var newPartner = await _repository.CreateUserAsync("pgrimskogPartner@lifecouple.net", "EA5CEE24-90F9-4804-A45A-AAAAAAAAAAAA", true, "Per-lc-Partner", "Grimskog-lc-Partner", null);
                newPartner.Gender = "f";
                newPartner.Relationship_Id = up.Relationship_Id;
                await _repository.SetUserProfileAsync(newPartner);
                //Add device Id = ddf6a5c9-7b55-4e71-a457-54c69ad0b77f
                var deviceId = await _repository.SetAppCenterDeviceDetailsAsync(new AppCenterDeviceDetail
                {
                    DeviceId = "ddf6a5c9-7b55-4e71-a457-54c69ad0b77f",
                    DTCreated = DateTimeOffset.UtcNow,
                    TypeOfDeviceOs = DeviceOsTypeEnum.Android,
                    DTLastUpdated = DateTimeOffset.UtcNow,
                    UserprofileId = newPartner.Id
                });


                //Added as a devtest user 180319
                var user = new UserProfile()
                {
                    PrimaryEmail = "dsiegel@lifecouple.net",
                    ExternalRefId = "0602AC5F-DCB9-4265-B529-CF43AAC4988E",
                    IsDevTest = true,
                };
                newUser = await _repository.CreateUserAsync(user.PrimaryEmail, user.ExternalRefId, user.IsDevTest, "dan", "siegel", null);

                //User registered in lcdevtestb2ctenant.onmicrosoft.com 180316
                user = new UserProfile()
                {
                    PrimaryEmail = "per@grimskog.com",
                    ExternalRefId = "73a9ef85-5b22-4f61-987f-7b566dbbf4fd",
                    IsDevTest = false,
                };
                newUser = await _repository.CreateUserAsync(user.PrimaryEmail, user.ExternalRefId, user.IsDevTest, "per", "grimskog", null);

                //Used for BusinessLogicTests
                user = new UserProfile()
                {
                    PrimaryEmail = "BusinessLogicTests@lifecouple.net",
                    ExternalRefId = "BusinessLogicTests",
                    IsDevTest = true,
                };
                newUser = await _repository.CreateUserAsync(user.PrimaryEmail, user.ExternalRefId, user.IsDevTest, "businessLogic", "Test", null);

            }
            System.Diagnostics.Debug.WriteLine("SeedUsersAsync() - done");
        }
    }
}
