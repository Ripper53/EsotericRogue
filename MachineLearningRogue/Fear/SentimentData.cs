using Microsoft.ML.Data;

namespace MachineLearningRogue.Fear {
    public class SentimentData {
        [LoadColumn(0)]
        public string Text { get; set; }
        [LoadColumn(1)]
        public bool Label { get; set; }
    }
}
