using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    
    class Movement
    {
        public double Speed;
        public double Turn;
    }

    public class PID
    {
        public double Kp { get; private set; }
        public double MaxOutput { get; private set; }

        public PID(double aKp, double maxOutput)
        {
            Kp = aKp;
            MaxOutput = maxOutput;
        }

        public double ValueForError(double error)
        {
            return Math.Min(Kp * error, MaxOutput);
        }

        /*
        private long lastTime;
        private float iTerm;
        private float lastInput;

        public float Input { get; set; }
        public float Output { get; set; }
        public float Setpoint { get; set; }

        
        public float Ki { get; private set; }
        public float Kd { get; private set; }

        public int SampleTime { get; set; }
        public float OutputMinimum { get; set; }
        public float OutputMaximum { get; set; }

        public void Compute()
        {
            // early exit if called before sample interval
            long now = DateTime.Now.Ticks;
            long timeDelta = now - lastTime;
            if (timeDelta < SampleTime) return;

            // computer working error
            float error = Setpoint - Input;
            iTerm += (Ki * error);

            // clamp intergral in output ranges
            if (iTerm > OutputMaximum) iTerm = OutputMaximum;
            if (iTerm < OutputMinimum) iTerm = OutputMinimum;

            // calculate input error factor
            float iError = Input - lastInput;

            // calculate PID Output
            Output = Kp * error + iTerm - Kd * iError;
            if (Output > OutputMaximum) Output = OutputMaximum;
            if (Output < OutputMinimum) Output = OutputMinimum;

            // replace interval variables for next iteration
            lastInput = Input;
            lastTime = now;
        }

        public void SetTunings(float Kp, float Ki, float Kd)
        {
            // error check for settings less than 0
            if (Kp < 0 || Ki < 0 || Kd < 0) return;

            // scale sample interval and apply to tuning
            float SampleRate = (float)SampleTime / 1000;
            this.Kp = Kp;
            this.Ki = Ki * SampleRate;
            this.Kd = Kd / SampleRate;

        }

        public void SetSampleTime(int NewSampleTime)
        {
            // new sample time must be valid
            if (NewSampleTime > 0)
            {
                // adjust scaling of intergral and derivative for new time
                float ratio = (float)NewSampleTime / (float)SampleTime;
                Ki *= ratio;
                Kd /= ratio;

                this.SampleTime = NewSampleTime;
            }
        }

        public void SetOutputLimits(float Min, float Max)
        {
            // min must be less than max
            if (Min > Max) return;
            this.OutputMinimum = Min;
            this.OutputMaximum = Max;

            // immediatly clamp the output value and intergral term
            if (Output > OutputMaximum) Output = OutputMaximum;
            if (Output < OutputMinimum) Output = OutputMinimum;

            if (iTerm > OutputMaximum) iTerm = OutputMaximum;
            if (iTerm < OutputMinimum) iTerm = OutputMinimum;
        }

        void Initialize()
        {
            lastInput = Input;
            iTerm = 0;
            if (iTerm > OutputMaximum) iTerm = OutputMaximum;
            if (iTerm < OutputMinimum) iTerm = OutputMinimum;
        }

        public PID(float Input, float Output, float Setpoint, float Kp, float Ki, float Kd)
        {
            this.Input = Input;
            this.Output = Output;
            this.Setpoint = Setpoint;
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;

            Initialize();
        }
         */
    }
}
