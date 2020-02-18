using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace MachineLearningRogue.Fear {
    public class FearTrainer : Trainer {

        public override Trained Run() {
            // Load Data
            const string path = "../../../Fear/Data.txt";
            IDataView dataView = MLContext.Data.LoadFromTextFile<SentimentData>(path);

            // Create data into readable data for neural net
            IEstimator<ITransformer> dataProcessPipline = MLContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text));

            // Training algorithm
            IEstimator<ITransformer> trainer = MLContext.BinaryClassification.Trainers.SdcaLogisticRegression();

            EstimatorChain<ITransformer> trainingPipline = dataProcessPipline.Append(trainer);

            //Console.WriteLine(trainingPipline.Preview(dataView));

            ITransformer trainedModel = trainingPipline.Fit(dataView);

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
