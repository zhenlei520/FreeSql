using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FreeSql.Tests.Firebird
{
    public class FirebirdCodeFirstTest
    {
        [Fact]
        public void InsertUpdateParameter()
        {
            var fsql = g.firebird;
            fsql.CodeFirst.SyncStructure<ts_iupstr_bak>();
            var item = new ts_iupstr { id = Guid.NewGuid(), title = string.Join(",", Enumerable.Range(0, 2000).Select(a => "�����й���")) };
            Assert.Equal(1, fsql.Insert(item).ExecuteAffrows());
            var find = fsql.Select<ts_iupstr>().Where(a => a.id == item.id).First();
            Assert.NotNull(find);
            Assert.Equal(find.id, item.id);
            Assert.Equal(find.title, item.title);
        }
        [Table(Name = "ts_iupstr_bak", DisableSyncStructure = true)]
        class ts_iupstr
        {
            public Guid id { get; set; }
            public string title { get; set; }
        }
        class ts_iupstr_bak
        {
            public Guid id { get; set; }
            [Column(StringLength = -1)]
            public string title { get; set; }
        }

        [Fact]
        public void Text_StringLength_1()
        {
            var str1 = string.Join(",", Enumerable.Range(0, 10000).Select(a => "�����й���"));

            var item1 = new TS_TEXT02 { Data = str1 };
            Assert.Equal(1, g.firebird.Insert(item1).ExecuteAffrows());

            var item2 = g.firebird.Select<TS_TEXT02>().Where(a => a.Id == item1.Id).First();
            Assert.Equal(str1, item2.Data);

            //NoneParameter
            item1 = new TS_TEXT02 { Data = str1 };
            Assert.Equal(1, g.firebird.Insert(item1).NoneParameter().ExecuteAffrows());
            item2 = g.firebird.Select<TS_TEXT02>().Where(a => a.Id == item1.Id).First();
            Assert.Equal(str1, item2.Data);
            //Assert.Throws<Exception>(() => g.firebird.Insert(item1).NoneParameter().ExecuteAffrows());
            //Dynamic SQL Error
            //SQL error code = -104
            //String literal with 159999 bytes exceeds the maximum length of 65535 bytes
        }
        class TS_TEXT02
        {
            public Guid Id { get; set; }
            [Column(StringLength = - 1)]
            public string Data { get; set; }
        }

        [Fact]
        public void Blob()
        {
            var str1 = string.Join(",", Enumerable.Range(0, 10000).Select(a => "�����й���"));
            var data1 = Encoding.UTF8.GetBytes(str1);

            var item1 = new TS_BLB01 { Data = data1 };
            Assert.Equal(1, g.firebird.Insert(item1).ExecuteAffrows());

            var item2 = g.firebird.Select<TS_BLB01>().Where(a => a.Id == item1.Id).First();
            Assert.Equal(item1.Data.Length, item2.Data.Length);
            var str2 = Encoding.UTF8.GetString(item2.Data);
            Assert.Equal(str1, str2);

            //NoneParameter
            item1 = new TS_BLB01 { Data = data1 };
            Assert.Equal(1, g.firebird.Insert(item1).NoneParameter().ExecuteAffrows());
            item2 = g.firebird.Select<TS_BLB01>().Where(a => a.Id == item1.Id).First();
            Assert.Equal(item1.Data.Length, item2.Data.Length);
            str2 = Encoding.UTF8.GetString(item2.Data);
            Assert.Equal(str1, str2);
            //Assert.Throws<Exception>(() => g.firebird.Insert(item1).NoneParameter().ExecuteAffrows());
            //Dynamic SQL Error
            //SQL error code = -104
            //String literal with 159999 bytes exceeds the maximum length of 65535 bytes
        }
        class TS_BLB01
        {
            public Guid Id { get; set; }
            public byte[] Data { get; set; }
        }
        [Fact]
        public void StringLength()
        {
            var dll = g.firebird.CodeFirst.GetComparisonDDLStatements<TS_SLTB>();
            g.firebird.CodeFirst.SyncStructure<TS_SLTB>();
        }
        class TS_SLTB
        {
            public Guid Id { get; set; }
            [Column(StringLength = 50)]
            public string Title { get; set; }

            [Column(IsNullable = false, StringLength = 50)]
            public string TitleSub { get; set; }
        }

        [Fact]
        public void ���ֱ�_�ֶ�()
        {
            var sql = g.firebird.CodeFirst.GetComparisonDDLStatements<�������ֱ�>();
            g.firebird.CodeFirst.SyncStructure<�������ֱ�>();

            var item = new �������ֱ�
            {
                ���� = "���Ա���",
                ����ʱ�� = DateTime.Now
            };
            Assert.Equal(1, g.firebird.Insert<�������ֱ�>().AppendData(item).ExecuteAffrows());
            Assert.NotEqual(Guid.Empty, item.���);
            var item2 = g.firebird.Select<�������ֱ�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);

            item.���� = "���Ա������";
            Assert.Equal(1, g.firebird.Update<�������ֱ�>().SetSource(item).ExecuteAffrows());
            item2 = g.firebird.Select<�������ֱ�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);

            item.���� = "���Ա������_repo";
            var repo = g.firebird.GetRepository<�������ֱ�>();
            Assert.Equal(1, repo.Update(item));
            item2 = g.firebird.Select<�������ֱ�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);

            item.���� = "���Ա������_repo22";
            Assert.Equal(1, repo.Update(item));
            item2 = g.firebird.Select<�������ֱ�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);
        }
        [Table(Name = "123tb")]
        [OraclePrimaryKeyName("pk1_123tb")]
        class �������ֱ�
        {
            [Column(IsPrimary = true, Name = "123id")]
            public Guid ��� { get; set; }

            [Column(Name = "123title")]
            public string ���� { get; set; }

            [Column(Name = "123time")]
            public DateTime ����ʱ�� { get; set; }
        }

        [Fact]
        public void ���ı�_�ֶ�()
        {
            var sql = g.firebird.CodeFirst.GetComparisonDDLStatements<�������ı�>();
            g.firebird.CodeFirst.SyncStructure<�������ı�>();

            var item = new �������ı�
            {
                ���� = "���Ա���",
                ����ʱ�� = DateTime.Now
            };
            Assert.Equal(1, g.firebird.Insert<�������ı�>().AppendData(item).ExecuteAffrows());
            Assert.NotEqual(Guid.Empty, item.���);
            var item2 = g.firebird.Select<�������ı�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);

            item.���� = "���Ա������";
            Assert.Equal(1, g.firebird.Update<�������ı�>().SetSource(item).ExecuteAffrows());
            item2 = g.firebird.Select<�������ı�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);

            item.���� = "���Ա������_repo";
            var repo = g.firebird.GetRepository<�������ı�>();
            Assert.Equal(1, repo.Update(item));
            item2 = g.firebird.Select<�������ı�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);

            item.���� = "���Ա������_repo22";
            Assert.Equal(1, repo.Update(item));
            item2 = g.firebird.Select<�������ı�>().Where(a => a.��� == item.���).First();
            Assert.NotNull(item2);
            Assert.Equal(item.���, item2.���);
            Assert.Equal(item.����, item2.����);
        }
        class �������ı�
        {
            [Column(IsPrimary = true)]
            public Guid ��� { get; set; }

            public string ���� { get; set; }

            [Column(ServerTime = DateTimeKind.Local, CanUpdate = false)]
            public DateTime ����ʱ�� { get; set; }

            [Column(ServerTime = DateTimeKind.Local)]
            public DateTime ����ʱ�� { get; set; }
        }

        [Fact]
        public void AddUniques()
        {
            var sql = g.firebird.CodeFirst.GetComparisonDDLStatements<AddUniquesInfo>();
            g.firebird.CodeFirst.SyncStructure<AddUniquesInfo>();
        }
        [Table(Name = "AddUniquesInfo", OldName = "AddUniquesInfo2")]
        [Index("uk_phone", "phone", true)]
        [Index("uk_group_index", "group,index", true)]
        [Index("uk_group_index2", "group,index", false)]
        class AddUniquesInfo
        {
            /// <summary>
            /// ���
            /// </summary>
            public Guid id { get; set; }
            public string phone { get; set; }

            public string group { get; set; }
            public int index { get; set; }
            public string index22 { get; set; }
        }
        [Fact]
        public void AddField()
        {
            var sql = g.firebird.CodeFirst.GetComparisonDDLStatements<TopicAddField>();

            var id = g.firebird.Insert<TopicAddField>().AppendData(new TopicAddField { }).ExecuteIdentity();

            //var inserted = g.firebird.Insert<TopicAddField>().AppendData(new TopicAddField { }).ExecuteInserted();
        }

        [Table(Name = "TopicAddField", OldName = "xxxtb.TopicAddField")]
        public class TopicAddField
        {
            [Column(IsIdentity = true)]
            public int Id { get; set; }

            public string name { get; set; }

            [Column(DbType = "varchar(200) not null", OldName = "title")]
            public string title2 { get; set; } = "10";

            [Column(IsIgnore = true)]
            public DateTime ct { get; set; } = DateTime.Now;
        }

        [Fact]
        public void GetComparisonDDLStatements()
        {
            var sql = g.firebird.CodeFirst.GetComparisonDDLStatements<TableAllType>();
        }

        IInsert<TableAllType> insert => g.firebird.Insert<TableAllType>();
        ISelect<TableAllType> select => g.firebird.Select<TableAllType>();

        [Fact]
        public void CurdAllField()
        {
            var item = new TableAllType { };
            insert.AppendData(item).ExecuteAffrows();

            var newitem = select.Where(a => a.Id == item.Id).ToOne();

            var item2 = new TableAllType
            {
                Bool = true,
                BoolNullable = true,
                Byte = 255,
                ByteNullable = 127,
                Bytes = Encoding.UTF8.GetBytes("�����й���"),
                DateTime = DateTime.Now,
                DateTimeNullable = DateTime.Now.AddHours(-1),
                Decimal = 99.99M,
                DecimalNullable = 99.98M,
                Double = 999.99,
                DoubleNullable = 999.98,
                Enum1 = TableAllTypeEnumType1.e5,
                Enum1Nullable = TableAllTypeEnumType1.e3,
                Enum2 = TableAllTypeEnumType2.f2,
                Enum2Nullable = TableAllTypeEnumType2.f3,
                Float = 19.99F,
                FloatNullable = 19.98F,
                Guid = Guid.NewGuid(),
                GuidNullable = Guid.NewGuid(),
                Int = int.MaxValue,
                IntNullable = int.MinValue,
                SByte = 100,
                SByteNullable = 99,
                Short = short.MaxValue,
                ShortNullable = short.MinValue,
                String = "�����й���string'\\?!@#$%^&*()_+{}}{~?><<>",
                TimeSpan = TimeSpan.FromSeconds(999),
                TimeSpanNullable = TimeSpan.FromSeconds(60),
                UInt = uint.MaxValue,
                UIntNullable = uint.MinValue,
                ULong = long.MaxValue,
                ULongNullable = ulong.MinValue,
                UShort = ushort.MaxValue,
                UShortNullable = ushort.MinValue,
                testFielLongNullable = long.MinValue
            };
            var sqlPar = insert.AppendData(item2).ToSql();
            var sqlText = insert.AppendData(item2).NoneParameter().ToSql();

            insert.AppendData(item2).ExecuteAffrows();
            var newitem2 = select.Where(a => a.Id == item2.Id).ToOne();
            Assert.Equal(item2.String, newitem2.String);

            item2.Id = Guid.NewGuid();
            insert.NoneParameter().AppendData(item2).ExecuteAffrows();
            newitem2 = select.Where(a => a.Id == item2.Id).ToOne();
            Assert.Equal(item2.String, newitem2.String);

            var items = select.ToList();
        }

        [Table(Name = "tb_alltype")]
        class TableAllType
        {
            [Column(IsPrimary = true)]
            public Guid Id { get; set; }

            public string id2 { get; set; } = "id2=10";

            public bool Bool { get; set; }
            public sbyte SByte { get; set; }
            public short Short { get; set; }
            public int Int { get; set; }
            public long Long { get; set; }
            public byte Byte { get; set; }
            public ushort UShort { get; set; }
            public uint UInt { get; set; }
            public ulong ULong { get; set; }
            public double Double { get; set; }
            public float Float { get; set; }
            public decimal Decimal { get; set; }
            public TimeSpan TimeSpan { get; set; }

            [Column(ServerTime = DateTimeKind.Local)]
            public DateTime DateTime { get; set; }
            [Column(ServerTime = DateTimeKind.Local)]
            public DateTime DateTimeOffSet { get; set; }

            public byte[] Bytes { get; set; }
            public string String { get; set; }
            public Guid Guid { get; set; }

            public bool? BoolNullable { get; set; }
            public sbyte? SByteNullable { get; set; }
            public short? ShortNullable { get; set; }
            public int? IntNullable { get; set; }
            public long? testFielLongNullable { get; set; }
            public byte? ByteNullable { get; set; }
            public ushort? UShortNullable { get; set; }
            public uint? UIntNullable { get; set; }
            public ulong? ULongNullable { get; set; }
            public double? DoubleNullable { get; set; }
            public float? FloatNullable { get; set; }
            public decimal? DecimalNullable { get; set; }
            public TimeSpan? TimeSpanNullable { get; set; }

            [Column(ServerTime = DateTimeKind.Local)]
            public DateTime? DateTimeNullable { get; set; }
            [Column(ServerTime = DateTimeKind.Local)]
            public DateTime? DateTimeOffSetNullable { get; set; }

            public Guid? GuidNullable { get; set; }

            public TableAllTypeEnumType1 Enum1 { get; set; }
            public TableAllTypeEnumType1? Enum1Nullable { get; set; }
            public TableAllTypeEnumType2 Enum2 { get; set; }
            public TableAllTypeEnumType2? Enum2Nullable { get; set; }
        }

        public enum TableAllTypeEnumType1 { e1, e2, e3, e5 }
        [Flags] public enum TableAllTypeEnumType2 { f1, f2, f3 }
    }
}
