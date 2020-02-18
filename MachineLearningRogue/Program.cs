using System;
using Microsoft.ML;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Trainers;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.CSharp;
using Microsoft.ML.Runtime;
using System.IO;
using MachineLearningRogue.Charm;

namespace MachineLearningRogue {
    class Program {

        static void Main(string[] args) {
            CharmTrainer charmTrainer = new CharmTrainer();
            Trainer.Trained trained = charmTrainer.Run();

            charmTrainer.MakePredictions(trained);
        }



    }
}
