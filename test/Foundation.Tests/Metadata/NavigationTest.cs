using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Metadata;
using Foundation.Metadata.Conventions;
using Foundation.Metadata.DataAnnotations;
using Xunit;

namespace Foundation.Tests.Metadata
{
    public class NavigationTest
    {
        #region Test 1

        private class E
        {
            public long Id { get; set; }
            public F NavToF { get; set; }
            public F NavToF_2 { get; set; }
        }

        private class F
        {
            public long Id { get; set; }
        }

        [Fact(DisplayName = "Ambiguous navigations : 2 nav, 0 inverse, 0 attribute")]
        public void Mulitple_navigation_properties_wihtout_attribute_create_ambiguous_annotations()
        {
            var EF_Model = new Model(new CoreConventionSetBuilder().CreateConventionSet());
            EF_Model.GetOrAddEntityForDebugMode(typeof(E));

            var annotations = EF_Model.FindEntity<E>().FindAnnotation(RelationshipDiscoveryConvention.AMBIGUOUS_NAVIGATIONS_ANNOTATION_NAME)?.Value as SortedDictionary<PropertyInfo, Type>;

            Assert.NotNull(annotations);
            Assert.True(annotations.Count == 2);
            Assert.Same(typeof(E).GetProperty(nameof(E.NavToF)), annotations.Keys.ToList()[0]);
            Assert.Same(typeof(E).GetProperty(nameof(E.NavToF_2)), annotations.Keys.ToList()[1]);
        }

        #endregion

        #region Test 2

        private class G
        {
            public long Id { get; set; }
            [Relationship("G_H", "NavToG")]
            public H NavToH { get; set; }
            [Relationship("G_H_2", "NavToG")]
            public H NavToH_2 { get; set; }
        }

        private class H
        {
            public long Id { get; set; }
            public G NavToG { get; set; }
        }

        [Fact(DisplayName = "Ambiguous navigation : 2 nav, 1 inverse, 2 attributes same inverse prop")]
        public void Mulitple_navigation_properties_targeting_the_same_inverse_property_create_ambiguous_annotation()
        {
            var GH_Model = new Model(new CoreConventionSetBuilder().CreateConventionSet());
            GH_Model.GetOrAddEntityForDebugMode(typeof(G));

            var annotations = GH_Model.FindEntity<G>().FindAnnotation(RelationshipDiscoveryConvention.AMBIGUOUS_NAVIGATIONS_ANNOTATION_NAME)?.Value as SortedDictionary<PropertyInfo, Type>;

            Assert.NotNull(annotations);
            Assert.True(annotations.Count == 1);
            Assert.Same(typeof(G).GetProperty(nameof(G.NavToH_2)), annotations.Keys.ToList()[0]);
        }

        #endregion

        private class I
        {
            public long Id { get; set; }
            public J NavToJ { get; set; }
        }

        private class J
        {
            public long Id { get; set; }
        }

        [Fact(DisplayName = "XXX")]
        public void XXX()
        {
            var model = new Model(new CoreConventionSetBuilder().CreateConventionSet());
            model.GetOrAddEntityForDebugMode(typeof(I));


        }
    }
}
