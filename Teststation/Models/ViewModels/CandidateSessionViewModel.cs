﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Teststation.Models
{
    public class CandidateSessionViewModel
    {
        public List<TestCandidateViewModel> Tests { get; set; }
        public UserInformation UserInformation { get; set; }
        public string WholeResult { get; set; }
        public string ResultText { get; set; }

        public CandidateSessionViewModel(Database _context, string userId)
        {
            GetUser(_context, userId);
            GetTests(_context);
            CalculateResult();
        }

        private void GetUser(Database _context, string userId)
        {
            UserInformation = _context.UserInformation.FirstOrDefault(x => x.UserId == userId);
            UserInformation.User = _context.Users.FirstOrDefault(x => x.Id == userId);
        }

        private void GetTests(Database _context)
        {
            Tests = new List<TestCandidateViewModel>();
            var allTests = _context.Tests
                .Where(x => x.ReleaseStatus == TestStatus.Public);

            foreach (var test in allTests)
            {
                var session = _context.Sessions.FirstOrDefault(x => x.TestId == test.Id && x.CandidateId == UserInformation.Id);
                var testRow = new TestCandidateViewModel
                {
                    Test = test,
                    IsStarted = session != null
                };

                if (testRow.IsStarted)
                {
                    var evaluation = new EvaluationViewModel(test, UserInformation.Id, _context);
                    testRow.Result = Consts.resultIfEvaluationHasErrors;
                    if (evaluation.Answers != null && evaluation.Answers.Count != 0)
                    {
                        try
                        {
                            testRow.Result = evaluation.GetPercentage();
                        }
                        catch
                        {
                            session.Completed = false;
                            _context.Update(session);
                            _context.SaveChanges();
                        }
                        
                    }
                    testRow.Duration = session.Duration;
                    testRow.Completed = session.Completed;
                }
                else
                {
                    testRow.Duration = new TimeSpan();
                    testRow.Result = 0;
                    testRow.Completed = false;
                }

                Tests.Add(testRow);
            }
        }

        private void CalculateResult()
        {
            var finishedTests = Tests
                .Where(x => x.Completed)
                .ToList();

            var ammountTests = finishedTests.Count;
            double resultSum = 0;
            foreach (var finishedTest in finishedTests)
            {
                resultSum += finishedTest.Result;
            }
            var result = (double)resultSum / (double)ammountTests;
                        
            if (result * 100 >= Consts.neededPercentage)
            {
                ResultText = Consts.goodGrade;
            }
            else
            {
                ResultText = Consts.badGrade;
            }
            WholeResult = result.ToString("P");
        }
    }
}
