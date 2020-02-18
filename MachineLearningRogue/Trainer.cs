using Microsoft.ML;

namespace MachineLearningRogue {
    public abstract class Trainer {
        public readonly MLContext MLContext = new MLContext();

        public abstract Trained Run();

        public class Trained {
            private readonly MLContext mlContext;
            private readonly ITransformer trainedModel;
            private readonly DataViewSchema schema;

            public Trained(MLContext mlContext, ITransformer trainedModel, DataViewSchema schema) {
                this.mlContext = mlContext;
                this.trainedModel = trainedModel;
                this.schema = schema;
            }

            public void Save(string path) {
                mlContext.Model.Save(trainedModel, schema, path);
            }

            public PredictionEngine<TSrc, TDst> GetPredictionEngine<TSrc, TDst>() where TSrc : class where TDst : class, new() {
                return mlContext.Model.CreatePredictionEngine<TSrc, TDst>(trainedModel);
            }
        }

    }
}
