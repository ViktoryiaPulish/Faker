using Xunit;
using System;
using System.Collections.Generic;
using Faker;

namespace Faker.Tests;

public class Student
{
    public string Name { get; }
    public int Age { get; }

    public Student(string name) { Name = name; }

    public Student(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

public class Course
{
    public Course RelatedCourse { get; set; } 
}

public class ComplexLogicTests
{
    private readonly Faker _faker = new Faker();

    [Fact]
    public void Create_GenericList_ReturnsPopulatedList()
    {
        List<int> numbers = _faker.Create<List<int>>();

        Assert.NotNull(numbers);
        Assert.NotEmpty(numbers);
    }

    [Fact]
    public void Create_RecursiveClass_HandlesCycleBySettingNull()
    {
        Course math = _faker.Create<Course>();

        Assert.NotNull(math);
        Assert.Null(math.RelatedCourse); 
    }

    [Fact]
    public void Create_ComplexObjectWithMultipleConstructors_UsesLargestOne()
    {
        Student student = _faker.Create<Student>();

        Assert.NotNull(student.Name);
        Assert.NotEqual(0, student.Age); 
    }

    [Fact]
    public void Create_NullableType_ReturnsValue()
    {
        int? value = _faker.Create<int?>();
        Assert.NotNull(value);
    }

    [Fact]
    public void Create_NestedList_Works()
    {
        var nestedList = _faker.Create<List<List<int>>>();
        Assert.NotNull(nestedList);
        Assert.NotEmpty(nestedList); 
        Assert.NotEmpty(nestedList[0]); 
    }
}