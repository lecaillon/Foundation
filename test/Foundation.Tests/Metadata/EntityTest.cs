using System.Linq;
using Foundation.Metadata;
using Foundation.Metadata.Conventions;
using Xunit;

namespace Foundation.Tests.Metadata
{
    public class EntityTest
    {
        Model ABCD_Model;

        //    A
        //   / \
        //  B   C
        //       \
        //        D

        private abstract class A
        {
            public int Id { get; set; }
            public string AProperty { get; set; }
            public int ReadOnlyProperty { get; }
        }

        private class B : A
        {
            public string BProperty { get; set; }
        }

        private class C : A
        {
            public string CProperty { get; set; }
        }

        private class D : C
        {
            public string DProperty { get; set; }
        }

        public EntityTest()
        {
            ABCD_Model = new Model(new CoreConventionSetBuilder().CreateConventionSet());
            ABCD_Model.GetOrAddEntityForDebugMode(typeof(B));
            ABCD_Model.GetOrAddEntityForDebugMode(typeof(D));
        }

        [Fact(DisplayName = "ABCD model has 4 entities")]
        public void ABCD_model_has_4_entities()
        {
            Assert.True(ABCD_Model.GetEntities().ToList().Count == 4, "4 entités (A,B,C,D) doivent faire partie de ce modèle.");
        }

        [Fact(DisplayName = "Entity A is abstract")]
        public void Entity_is_abstract_when_type_is_abstract()
        {
            Assert.True(ABCD_Model.FindEntity<A>().IsAbstract);
        }

        [Fact(DisplayName = "Entities B,C have BaseType = A")]
        public void Derived_type_must_have_a_base_type()
        {
            Assert.Equal(typeof(A), ABCD_Model.FindEntity<B>().BaseType.ClrType);
            Assert.Equal(typeof(A), ABCD_Model.FindEntity<C>().BaseType.ClrType);
            Assert.Equal(typeof(C), ABCD_Model.FindEntity<D>().BaseType.ClrType);
        }

        [Fact(DisplayName = "Entity A has no BaseType")]
        public void Base_type_is_null_when_entity_is_not_derived()
        {
            Assert.Null(ABCD_Model.FindEntity(typeof(A)).BaseType);
        }

        [Fact(DisplayName = "Entities B,D have no DirectlyDerivedEntities")]
        public void DirectlyDerivedEntities_is_empty_when_entity_has_not_derived_entity()
        {
            Assert.Empty(ABCD_Model.FindEntity<B>().GetDirectlyDerivedEntities());
            Assert.Empty(ABCD_Model.FindEntity<D>().GetDirectlyDerivedEntities());
        }

        [Fact(DisplayName = "Entity A have DirectlyDerivedEntities B,C")]
        public void DirectlyDerivedEntities_is_not_empty_when_entity_has_derived_entities()
        {
            Assert.Single(ABCD_Model.FindEntity<A>().GetDirectlyDerivedEntities(), ABCD_Model.FindEntity<B>());
            Assert.Single(ABCD_Model.FindEntity<A>().GetDirectlyDerivedEntities(), ABCD_Model.FindEntity<C>());
            Assert.Single(ABCD_Model.FindEntity<C>().GetDirectlyDerivedEntities(), ABCD_Model.FindEntity<D>());
        }

        [Fact(DisplayName = "Entity D has only 1 declared property = D")]
        public void Declared_properties_are_owned_by_the_declaring_type_only()
        {
            Assert.NotNull(ABCD_Model.FindEntity<D>().FindDeclaredProperty("DProperty"));
            Assert.Null(ABCD_Model.FindEntity<D>().FindDeclaredProperty("CProperty"));
            Assert.Null(ABCD_Model.FindEntity<D>().FindDeclaredProperty("AProperty"));
        }

        [Fact(DisplayName = "Entity D has properties D,C,A")]
        public void Properties_returns_all_the_properties_included_inherited_properties()
        {
            Assert.NotNull(ABCD_Model.FindEntity<D>().FindProperty("DProperty"));
            Assert.NotNull(ABCD_Model.FindEntity<D>().FindProperty("CProperty"));
            Assert.NotNull(ABCD_Model.FindEntity<D>().FindProperty("AProperty"));
        }

        [Fact(DisplayName= "ReadOnlyProperty is not mapped")]
        public void Read_only_property_is_not_mapped()
        {
            Assert.Null(ABCD_Model.FindEntity<A>().FindProperty("ReadOnlyProperty"));
            Assert.Null(ABCD_Model.FindEntity<C>().FindProperty("ReadOnlyProperty"));
            Assert.Null(ABCD_Model.FindEntity<D>().FindProperty("ReadOnlyProperty"));
        }

        [Fact(DisplayName = "Entity A PK is property Id")]
        public void PrimaryKey_convention_is_Id()
        {
            Assert.Equal(ABCD_Model.FindEntity<A>().FindProperty("Id").PrimaryKey, ABCD_Model.FindEntity<A>().FindDeclaredPrimaryKey());
            Assert.Equal(ABCD_Model.FindEntity<A>().FindProperty("Id").PrimaryKey, ABCD_Model.FindEntity<A>().FindPrimaryKey());
            Assert.Equal(ABCD_Model.FindEntity<A>(), ABCD_Model.FindEntity<A>().FindPrimaryKey().DeclaringEntity);
        }

        [Fact(DisplayName = "Entity D PK is property Id of Entity A")]
        public void PrimaryKey_is_inherited_when_entity_is_derived()
        {
            Assert.Null(ABCD_Model.FindEntity<D>().FindDeclaredPrimaryKey());
            Assert.Equal(ABCD_Model.FindEntity<A>().FindProperty("Id").PrimaryKey, ABCD_Model.FindEntity<D>().FindPrimaryKey());
            Assert.Equal(ABCD_Model.FindEntity<A>(), ABCD_Model.FindEntity<D>().FindPrimaryKey().DeclaringEntity);
        }
    }
}
