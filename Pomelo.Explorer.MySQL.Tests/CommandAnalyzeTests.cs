using System;
using System.Linq;
using Xunit;

namespace Pomelo.Explorer.MySQL.Tests
{
    public class CommandAnalyzeTests
    {
        [Fact]
        public void EmptyCommandTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("");
            Assert.False(result.IsSimpleSelect);
        }

        [Fact]
        public void SelectAllTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select * from `user` where id = 1");
            Assert.True(result.IsSimpleSelect);
            Assert.Single(result.Columns);
            Assert.Equal("*", result.Columns.First());
            Assert.Equal("user", result.Table);
        }

        [Fact]
        public void SelectSomeTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select `id`, username, password, `email` from user where id = 1");
            Assert.True(result.IsSimpleSelect);
            Assert.Equal(4, result.Columns.Count);
            Assert.Equal("id", result.Columns[0]);
            Assert.Equal("username", result.Columns[1]);
            Assert.Equal("password", result.Columns[2]);
            Assert.Equal("email", result.Columns[3]);
            Assert.Equal("user", result.Table);
        }

        [Fact]
        public void SelectJoinTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select `id`, username, password, `email` from user inner join `test` on `user`.id = `test`.`userid`");
            Assert.False(result.IsSimpleSelect);
        }

        [Fact]
        public void SelectVariableTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select @test");
            Assert.False(result.IsSimpleSelect);
        }

        [Fact]
        public void NonSelectCommandTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("show tables");
            Assert.False(result.IsSimpleSelect);
        }

        [Fact]
        public void SelectAsTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select `id` as `Identifier`, `id` from user where id = 1");
            Assert.True(result.IsSimpleSelect);
            Assert.Equal("Identifier", result.Columns[0]);
        }

        [Fact]
        public void SelectCalcTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select `id` + 1, `id` from user where id = 1");
            Assert.True(result.IsSimpleSelect);
            Assert.Equal("`id` + 1", result.Columns[0]);
            Assert.Equal("id", result.Columns[1]);
        }

        [Fact]
        public void SelectFuncTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select FUNC(`id`) from user where id = 1");
            Assert.True(result.IsSimpleSelect);
            Assert.Equal("FUNC(`id`)", result.Columns[0]);
        }

        [Fact]
        public void SelectGroupByTest()
        {
            var result = MySqlCommandSpliter.AnalyzeCommand("select count(1), `group` from user group by `group`");
            Assert.False(result.IsSimpleSelect);
        }
    }
}
