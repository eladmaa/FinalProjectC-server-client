﻿using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Telhai.CS.APIServer.Models
{
    public class Exam
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }

        // ID, name, content
        [JsonPropertyName("examName")]
        public string examName { get; set; }
        [JsonPropertyName("examId")]
        public string examId { get; set; }
        [JsonPropertyName("DateTime")]
        public DateTime date { get; set; }
        [JsonPropertyName("TeacherName")]
        public string TeacherName { get; set; }
        [JsonPropertyName("BeginTime")]

        public DateTime BeginTime { get; set; }
        [JsonPropertyName("duration")]

        public float duration { get; set; }
        [JsonPropertyName("isRandom")]

        public bool isRandom { get; set; }
        [JsonPropertyName("questions")]

        public List<Question> questions { get; set; }
        
        public Exam(string examName, string examId)
        {
            this.examName = examName;
            this.examId = examId;
          //  this.id = Guid.NewGuid().ToString();
        }
        
        public Exam() 
        {
            examName = "";
            this.examId = Guid.NewGuid().ToString();
        }

        public Exam(string examName, string examId, DateTime date, string teacherName, DateTime beginTime, float duration, bool isRandom, List<Question> questions) : this(examName, examId)
        {
            this.date = date;
            TeacherName = teacherName;
            BeginTime = beginTime;
            this.duration = duration;
            this.isRandom = isRandom;
            this.questions = questions;
        }
    }

    
}
