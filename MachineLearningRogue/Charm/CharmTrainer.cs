using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace MachineLearningRogue.Charm {
    public class CharmTrainer : Trainer {

        private IDataView GetDataView(string path) {
            return MLContext.Data.LoadFromTextFile<SentimentData>(path);
        }

        public override Trained Run() {
            // Load Data
            const string path = "../../../Charm/Data.txt";
            IDataView dataView = GetDataView(path);

            // Create data into readable data for neural net
            IEstimator<ITransformer> dataProcessPipline = MLContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text));

            // Training algorithm
            IEstimator<ITransformer> trainer = MLContext.BinaryClassification.Trainers.SdcaLogisticRegression();

            EstimatorChain<ITransformer> trainingPipline = dataProcessPipline.Append(trainer);

            //Console.WriteLine(trainingPipline.Preview(dataView));

            ITransformer trainedModel = trainingPipline.Fit(dataView);

            // PREDICT AKA TESTING
            const string testPath = "../../../Charm/TestData.txt";
            IDataView testDataView = GetDataView(testPath);
            //Console.WriteLine(trainingPipline.Preview(testDataView));
            IDataView predictions = trainedModel.Transform(testDataView);
            CalibratedBinaryClassificationMetrics metrics = MLContext.BinaryClassification.Evaluate(predictions);

            return new Trained(MLContext, trainedModel, dataView.Schema);
        }

        public void MakePredictions(Trained trained) {
            using var pred = trained.GetPredictionEngine<SentimentData, OutputData>();
            while (true) {
                Console.WriteLine("Input:");
                string text = Console.ReadLine();
                OutputData output = pred.Predict(new SentimentData() {
                    Text = text
                });
                Console.WriteLine(output.Probability);
            }
        }

    }
}
