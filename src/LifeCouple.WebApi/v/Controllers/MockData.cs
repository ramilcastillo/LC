using LifeCouple.DTO.v;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.v.Controllers
{
    public class MockData
    {
        public static OnBoardingTemplateResponseInfo GetSampleData_Week_One_v1_1()
        {
            var text = "Move the slider to indicate your satisfaction level";

            var result = new OnBoardingTemplateResponseInfo();
            result.Id = "E069EF39-571C-4F65-A2FE-43A05B0BAB00";
            result.QuestionnaireSets = new List<QuestionnaireSetResponseInfo>();

            var questionsFor1stSet = new List<QuestionResponseInfo>();

            result.QuestionnaireSets.Add(new QuestionnaireSetResponseInfo
            {
                Id = "E069EF39-571C-4F65-A2FE-43A05B0BAB01",
                NextButtonText = "BEGIN QUESTIONNAIRE",
                Text = "This is an important step used to understand your relationship dynamics and to help craft your custom experience in LifeCouple.  The questionnaire will take approximately 10-15 minutes.",
                Title = "Onboarding questions",
                Questions = questionsFor1stSet
            });

            #region 1stSet
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a01",
                Title = "Affection",
                Description = @"Affection takes the loving relationship between a man and woman into the deeper realm of tender expressions. LifeCouple views affection as giving words of affirmation, acts of service, gifts, and physical touch.

Using this or your definition of affection.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a02",
                Title = "Intimacy",
                Description = @"LifeCouple’s view is an interpersonal relationship that involves physical plus emotional intimacy. Physical intimacy is characterized by friendship, platonic love, romantic love, and enjoyable sexual activity.

Using this or your definition of a sexual relationship.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a03",
                Title = "Emotional Connection",
                Description = @"LifeCouple’s view is a secure attachment, 'feeling in touch', 'can I get your attention when I need it', 'can you comfort me when needed', 'do you care about my well-being even when I am not around.' 

Using this or your definition of emotional connection.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a04",
                Title = "Empathy",
                Description = @"LifeCouple views empathy as putting yourself in the shoes of another to understand, be sensitive and share the feelings of another. 

Using this or your definition of empathy.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a05",
                Title = "Knowing and Understanding your Partner",
                Description = @"LifeCouple views knowing your partner as learning, showing interest, taking effort on important aspects of the relationship.  Understanding is truly listening and ultimately understanding on another’s needs and what matters to each of you. 

Using this or your definition of knowing and understanding your partner.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a06",
                Title = "Admiration",
                Description = @"Lifecouple views admiration as feeling respect, esteem and approval of one’s partner. 

Using this or your definition of admiration.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a07",
                Title = "Appreciation",
                Description = @"Lifecouple views appreciation as thankful recognition and gratitude towards one’s partner. 

Using this or your definition of appreciation.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a08",
                Title = "Daily Chores",
                Description = @"LifeCouple views daily chores as the management of daily, weekly and monthly duties involved in the running of a household, such as cleaning, cooking, home maintenance, shopping, and doing laundry. 

Using this or your definition of daily chores.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a09",
                Title = "Responsibilities",
                Description = @"LifeCouple views responsibilities as larger scale aspects such as dividing how to meet all of the children’s needs, planning for major events, managing finances, etc.  

Using this or your definition of responsibilities.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a10",
                Title = "Communication",
                Description = @"LifeCouple views communication as how you explain to your partner what you are experiencing and what your needs are and then having your partner affirm what you communicated. 

Using this or your definition of communication.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a11",
                Title = "Commitment",
                Description = @"LifeCouple views commitment as the effort to another involving love, trust, honesty, openness, other aspects that are important to both of you that includes willingness to do whatever it takes to make the relationship work. 

Using this or your definition of commitment.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a12",
                Title = "Trust",
                Description = @"LifeCouple views trust as a decision to love, be faithful, be reliable, and make each other feel safe, both emotionally and physically; and ultimately to make your relationship a top priority. 

Using this or your definition of trust.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a13",
                Title = "Healthy Conflict ",
                Description = @"LifeCouple’s view is that when disagreements arise in a relationship, healthy conflict includes deciding to handle the conflict in cooperative manner even if both sides do not agree and not resorting to attacking, fleeing or withdrawing. 

Using this or your definition of healthy conflict.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a14",
                Title = "Quality \"Couple Time\"",
                Description = @"LifeCouple’s view is sharing quality time together that includes a shared activity and interacting together as a couple without kids or other interruptions that is meaningful for the relationship. 

Using this or your definition of couple time.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a15",
                Title = "Support",
                Description = @"LifeCouple’s view includes supporting each other’s careers and goals as well as supporting each other in rough times. 

Using this or your definition of support.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a16",
                Title = "Respect",
                Description = @"LifeCouple’s view includes a positive feeling and action in a way that shows that you are aware of your partner’s needs and feelings and honoring your partner by exhibiting care, concern and consideration.

Using this or your definition of respect.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a17",
                Title = "Decisions",
                Description = @"LifeCouple’s view is that relationships have important decisions that are made over time including where to live, having children, working/staying at home with children, parenting styles. 

Using this or your definition of decisions.",
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a18",
                Title = @"Indicate your satisfaction level in your overall relationship.",
                Description = null,
                Text = text,
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null
            });

            foreach (var item in questionsFor1stSet)
            {
                if (item.TypeOfQuestion == QuestionType.Range)
                {
                    item.MaxRange = item.MaxRange ?? 10;
                    item.MinRange = item.MinRange ?? 0;
                }
            }

            #endregion



            return result;
        }


        public static OnBoardingTemplateResponseInfo GetSampleData()
        {
            var result = new OnBoardingTemplateResponseInfo();
            result.Id = "E069EF39-571C-4F65-A2FE-43A05B0BAB00";
            result.QuestionnaireSets = new List<QuestionnaireSetResponseInfo>();

            var questionsFor1stSet = new List<QuestionResponseInfo>();

            #region 1stSet
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a01",
                Text = "How important is it to you, how dedicated are you to put in effort in this program?",
                TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Very Important", Value = "a" },
                            new AnswerOptionsResponseInfo {
                                Text = "Somehow Important", Value = "b" },
                            new AnswerOptionsResponseInfo {
                                Text = "Not Very Important", Value = "c" },
                        }
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a02",
                Title = "Affection",
                Text = @"Affection takes the loving relationship between a man and woman into the deeper realm of tender expressions. LifeCouple views affection as giving words of affirmation, acts of service, gifts, and physical touch.

Using this or your definition of affection, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a03",
                Title = "Sexual Relationship",
                Text = @"LifeCouple’s view is an interpersonal relationship that involves physical plus emotional intimacy. Physical intimacy is characterized by friendship, platonic love, romantic love, and enjoyable sexual activity.

Using this or your definition of a sexual relationship, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a04",
                Title = "Emotional Connection",
                Text = @"LifeCouple’s view is a secure attachment, 'feeling in touch', 'can I get your attention when I need it', 'can you comfort me when needed', 'do you care about my well-being even when I am not around.' 

Using this or your definition of emotional connection, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a05",
                Title = "Empathy",
                Text = @"LifeCouple views empathy as putting yourself in the shoes of another to understand, be sensitive and share the feelings of another. 

Using this or your definition of empathy, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a06",
                Title = "Knowing and Understanding your Partner",
                Text = @"LifeCouple views knowing your partner as learning, showing interest, taking effort on important aspects of the relationship.  Understanding is truly listening and ultimately understanding on another’s needs and what matters to each of you. 

Using this or your definition of knowing and understanding your partner, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a07",
                Title = "Admiration",
                Text = @"Lifecouple views admiration as feeling respect, esteem and approval of one’s partner. 

Using this or your definition of admiration, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a08",
                Title = "Appreciation",
                Text = @"Lifecouple views appreciation as thankful recognition and gratitude towards one’s partner. 

Using this or your definition of appreciation, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a09",
                Title = "Daily Chores",
                Text = @"LifeCouple views daily chores as the management of daily, weekly and monthly duties involved in the running of a household, such as cleaning, cooking, home maintenance, shopping, and doing laundry. 

Using this or your definition of daily chores, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a10",
                Title = "Responsibilities",
                Text = @"LifeCouple views responsibilities as larger scale aspects such as dividing how to meet all of the children’s needs, planning for major events, managing finances, etc.  

Using this or your definition of responsibilities, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a11",
                Title = "Communication",
                Text = @"LifeCouple views communication as how you explain to your partner what you are experiencing and what your needs are and then having your partner affirm what you communicated. 

Using this or your definition of communication, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a12",
                Title = "Commitment",
                Text = @"LifeCouple views commitment as the effort to another involving love, trust, honesty, openness, other aspects that are important to both of you that includes willingness to do whatever it takes to make the relationship work. 

Using this or your definition of commitment, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a13",
                Title = "Trust",
                Text = @"LifeCouple views trust as a decision to love, be faithful, be reliable, and make each other feel safe, both emotionally and physically; and ultimately to make your relationship a top priority. 

Using this or your definition of trust, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a14",
                Title = "Healthy Conflict ",
                Text = @"LifeCouple’s view is that when disagreements arise in a relationship, healthy conflict includes deciding to handle the conflict in cooperative manner even if both sides do not agree and not resorting to attacking, fleeing or withdrawing. 

Using this or your definition of healthy conflict, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a15",
                Title = "Quality 'Couple Time'",
                Text = @"LifeCouple’s view is sharing quality time together that includes a shared activity and interacting together as a couple without kids or other interruptions that is meaningful for the relationship. 

Using this or your definition of couple time, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a16",
                Title = "Support",
                Text = @"LifeCouple’s view includes supporting each other’s careers and goals as well as supporting each other in rough times. 

Using this or your definition of support, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });
            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a17",
                Title = "Respect",
                Text = @"LifeCouple’s view includes a positive feeling and action in a way that shows that you are aware of your partner’s needs and feelings and honoring your partner by exhibiting care, concern and consideration.

Using this or your definition of respect, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });

            questionsFor1stSet.Add(new QuestionResponseInfo
            {
                Id = "a18",
                Title = "Decisions",
                Text = @"LifeCouple’s view is that relationships have important decisions that are made over time including where to live, having children, working/staying at home with children, parenting styles. 

Using this or your definition of decisions, move the slider to indicate your satisfaction level.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null,
            });


            result.QuestionnaireSets.Add(new QuestionnaireSetResponseInfo
            {
                Id = "E069EF39-571C-4F65-A2FE-43A05B0BAB01",
                NextButtonText = "BEGIN QUESTIONNAIRE",
                Text = "This is an important step used to understand your relationship dynamics and to help craft your custom experience in LC90.  The questionnaire will take approximately 15 minutes.",
                Title = "Onboarding questions",
                Questions = questionsFor1stSet
            });
            #endregion

            var questionsFor2ndSet = new List<QuestionResponseInfo>();

            #region 2ndSet
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b01",
                Title = null,
                Text = @"Are finances causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y",
                                ChildQuestions = new QuestionResponseInfo {
                                    Id = null,
                                    Text = "When It comes to an issue dealing with finances, are you willing to take action on this issue at this time?",
                                    TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                                    Title = null,
                                    AnswerOptions = new List<AnswerOptionsResponseInfo> {
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take any action", Value = "a" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take most actions", Value = "b" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take some action", Value = "c" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take little action", Value = "d" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take no action", Value = "e" },
                                    }
                                },
                            },
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b02",
                Title = null,
                Text = @"Are In-Laws causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y",
                                ChildQuestions = new QuestionResponseInfo {
                                    Id = null,
                                    Text = "When It comes to an issue dealing with this, are you willing to take action on this issue at this time?",
                                    TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                                    Title = null,
                                    AnswerOptions = new List<AnswerOptionsResponseInfo> {
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take any action", Value = "a" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take most actions", Value = "b" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take some action", Value = "c" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take little action", Value = "d" },
                                        new AnswerOptionsResponseInfo{ Text = "Willing to take no action", Value = "e" },
                                    }
                                },
                            },
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b03",
                Title = null,
                Text = @"Is drinking or any vices, legal or illegal, causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b04",
                Title = null,
                Text = @"Are any aspects regarding your friends or co-workers causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b05",
                Title = null,
                Text = @"Is talking in a negative tone and or talking down causing issues in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b06",
                Title = null,
                Text = @"Is spending time apart or not making an effort to spend time together causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b07",
                Title = null,
                Text = @"Is acting in a controlling way causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b08",
                Title = null,
                Text = @"Is acting with contempt (disregarding your partner’s concerns, rolling your eyes, etc.) causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b09",
                Title = null,
                Text = @"Is acting defensively causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b10",
                Title = null,
                Text = @"Is withdrawing and being unresponsive in conversations causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b11",
                Title = null,
                Text = @"Is playing the victim or not lowering your guard causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b12",
                Title = null,
                Text = @"Is unrealistic expectations causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });

            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b13",
                Title = null,
                Text = @"Is a work situation causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b14",
                Title = null,
                Text = @"Is a situation with criticizing or nagging communication causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b15",
                Title = null,
                Text = @"Is a situation with avoiding discussions involving feelings causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b16",
                Title = null,
                Text = @"Is a situation about not encouraging your partner causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b17",
                Title = null,
                Text = @"Is a situation regarding holding in hurt and becoming resentful causing an issue in your relationship?",
                TypeOfQuestion = QuestionType.YesNo,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                            new AnswerOptionsResponseInfo {
                                Text = "Yes", Value = "y"},
                            new AnswerOptionsResponseInfo {
                                Text = "No", Value = "n" },
                        }
            });
            questionsFor2ndSet.Add(new QuestionResponseInfo
            {
                Id = "b18",
                Title = null,
                Text = @"Indicate your satisfaction level in your overall relationship.",
                TypeOfQuestion = QuestionType.Range,
                AnswerOptions = null
            });
            #endregion

            result.QuestionnaireSets.Add(new QuestionnaireSetResponseInfo
            {
                Id = "E069EF39-571C-4F65-A2FE-43A05B0BAB02",
                NextButtonText = "Next",
                Text = "This next set of questions are 'situation' based. If an issue described occurs in your relationship, indicate Yes.  If the situation does not exist or exists but does not cause an issue in your relationship, indicate No.",
                Title = "Your almost done",
                Questions = questionsFor2ndSet
            });

            var questionsFor3rdSet = new List<QuestionResponseInfo>();

            #region 3rdSet
            questionsFor3rdSet.Add(new QuestionResponseInfo
            {
                Id = "c01",
                Title = null,
                Text = @"What was your total household income before taxes last year?",
                TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                    new AnswerOptionsResponseInfo{ Text = "Less than $49,000", Value = "a" },
                    new AnswerOptionsResponseInfo{ Text = "$50,000 to $74,999", Value = "b" },
                    new AnswerOptionsResponseInfo{ Text = "$75,000 to $99,999", Value = "c" },
                    new AnswerOptionsResponseInfo{ Text = "$100,000 to $150,000", Value = "d" },
                    new AnswerOptionsResponseInfo{ Text = "$100,000 to $150,000", Value = "e" },
                }
            });
            questionsFor3rdSet.Add(new QuestionResponseInfo
            {
                Id = "c02",
                Title = null,
                Text = @"To which do you most identify:",
                TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                    new AnswerOptionsResponseInfo{ Text = "Asian", Value = "a" },
                    new AnswerOptionsResponseInfo{ Text = "Black / African American", Value = "b" },
                    new AnswerOptionsResponseInfo{ Text = "Hispanic / Latino", Value = "c" },
                    new AnswerOptionsResponseInfo{ Text = "Native American", Value = "d" },
                    new AnswerOptionsResponseInfo{ Text = "Native Hawaiian/Pacific Islander", Value = "e" },
                    new AnswerOptionsResponseInfo{ Text = "White/Caucasian", Value = "f" },
                    new AnswerOptionsResponseInfo{ Text = "Other", Value = "g" },
                }
            });
            questionsFor3rdSet.Add(new QuestionResponseInfo
            {
                Id = "c03",
                Title = null,
                Text = @"What is the highest degree or level of education you have completed?",
                TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                    new AnswerOptionsResponseInfo{ Text = "Less than a high school diploma", Value = "a" },
                    new AnswerOptionsResponseInfo{ Text = "High school degree/ equivalent", Value = "b" },
                    new AnswerOptionsResponseInfo{ Text = "Some college, no degree", Value = "c" },
                    new AnswerOptionsResponseInfo{ Text = "Associate degree", Value = "d" },
                    new AnswerOptionsResponseInfo{ Text = "Bachelor’s degree ", Value = "e" },
                }
            }); questionsFor3rdSet.Add(new QuestionResponseInfo
            {
                Id = "c04",
                Title = null,
                Text = @"What is your current employment status?",
                TypeOfQuestion = QuestionType.MultipleOptionsSingleChoice,
                AnswerOptions = new List<AnswerOptionsResponseInfo> {
                    new AnswerOptionsResponseInfo{ Text = "Employed full time", Value = "a" },
                    new AnswerOptionsResponseInfo{ Text = "Unemployed", Value = "b" },
                    new AnswerOptionsResponseInfo{ Text = "Student", Value = "c" },
                    new AnswerOptionsResponseInfo{ Text = "Retired", Value = "d" },
                    new AnswerOptionsResponseInfo{ Text = "Homemaker / Stay at home with children", Value = "e" },
                    new AnswerOptionsResponseInfo{ Text = "Self-employed", Value = "f"},
                }
            });
            #endregion

            result.QuestionnaireSets.Add(new QuestionnaireSetResponseInfo
            {
                Id = "E069EF39-571C-4F65-A2FE-43A05B0BAB03",
                NextButtonText = "Next",
                Text = "To help LifeCouple provide you with a gauge of your relationship relative to other couples who have participated in LC90, there are a few more questions.",
                Title = "Onboarding questions",
                Questions = questionsFor3rdSet
            });

            return result;
        }


    }
}
