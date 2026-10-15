import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ArrowRight, AlertCircle } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { UserStage, UserObjective } from '../types';

export const RegisterPage: React.FC = () => {
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [currentStage, setCurrentStage] = useState<UserStage>('Idea');
  const [primaryObjective, setPrimaryObjective] = useState<UserObjective>('BuildProduct');
  const [hoursPerWeek, setHoursPerWeek] = useState(10);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const success = await register({
        fullName,
        email,
        password,
        currentStage,
        primaryObjective,
        hoursAvailablePerWeek: Number(hoursPerWeek),
      });

      if (success) {
        navigate('/onboarding');
      } else {
        setError('Registration failed. Email may already be in use.');
      }
    } catch {
      setError('An error occurred during registration.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-lg mx-auto py-12 px-4">
      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-sm space-y-6">
        <div className="text-center space-y-1">
          <h1 className="text-2xl font-bold text-stone-950">Join Builder Journey</h1>
          <p className="text-stone-500 text-xs">
            Start learning from empirical founder paths and ground your milestones in real evidence
          </p>
        </div>

        {error && (
          <div className="p-3 rounded-lg bg-rose-50 border border-rose-200 text-rose-800 text-xs flex items-center gap-2">
            <AlertCircle className="w-4 h-4 text-rose-600 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-1">
            <label className="block text-xs font-medium text-stone-700">Full Name</label>
            <input
              type="text"
              required
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="Alex Carter"
              className="w-full text-sm px-3 py-2 rounded-lg border border-stone-200 focus:outline-none focus:ring-2 focus:ring-stone-400 bg-stone-50"
            />
          </div>

          <div className="space-y-1">
            <label className="block text-xs font-medium text-stone-700">Email Address</label>
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="alex@example.com"
              className="w-full text-sm px-3 py-2 rounded-lg border border-stone-200 focus:outline-none focus:ring-2 focus:ring-stone-400 bg-stone-50"
            />
          </div>

          <div className="space-y-1">
            <label className="block text-xs font-medium text-stone-700">Password</label>
            <input
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              className="w-full text-sm px-3 py-2 rounded-lg border border-stone-200 focus:outline-none focus:ring-2 focus:ring-stone-400 bg-stone-50"
            />
          </div>

          <div className="grid grid-cols-2 gap-3 pt-2">
            <div className="space-y-1">
              <label className="block text-xs font-medium text-stone-700">Current Stage</label>
              <select
                value={currentStage}
                onChange={(e) => setCurrentStage(e.target.value as UserStage)}
                className="w-full text-xs px-2.5 py-2 rounded-lg border border-stone-200 bg-stone-50 focus:outline-none focus:ring-2 focus:ring-stone-400"
              >
                <option value="Idea">Idea Validation</option>
                <option value="Prototype">Building Prototype</option>
                <option value="Launched">Launched Product</option>
                <option value="FirstUsers">First Customers</option>
                <option value="Growth">Scaling & Growth</option>
              </select>
            </div>

            <div className="space-y-1">
              <label className="block text-xs font-medium text-stone-700">Primary Goal</label>
              <select
                value={primaryObjective}
                onChange={(e) => setPrimaryObjective(e.target.value as UserObjective)}
                className="w-full text-xs px-2.5 py-2 rounded-lg border border-stone-200 bg-stone-50 focus:outline-none focus:ring-2 focus:ring-stone-400"
              >
                <option value="BuildProduct">Ship Product</option>
                <option value="AcquireUsers">Get Customers</option>
                <option value="ImproveContent">Level Up Content</option>
                <option value="GrowAudience">Grow Audience</option>
              </select>
            </div>
          </div>

          <div className="space-y-1 pt-1">
            <label className="block text-xs font-medium text-stone-700">
              Hours Available Per Week ({hoursPerWeek} hrs)
            </label>
            <input
              type="range"
              min="2"
              max="60"
              step="2"
              value={hoursPerWeek}
              onChange={(e) => setHoursPerWeek(Number(e.target.value))}
              className="w-full accent-stone-900"
            />
            <div className="flex justify-between text-[10px] text-stone-400 font-mono">
              <span>Side-project (5h)</span>
              <span>Part-time (20h)</span>
              <span>Full-time (40h+)</span>
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-2.5 bg-stone-950 text-stone-50 font-medium rounded-lg hover:bg-stone-800 transition-colors text-sm flex items-center justify-center gap-2 mt-4"
          >
            {loading ? 'Creating Account...' : 'Complete Registration'}
            <ArrowRight className="w-4 h-4" />
          </button>
        </form>

        <div className="text-center text-xs text-stone-500 pt-2 border-t border-stone-100">
          Already have an account?{' '}
          <Link to="/login" className="text-brand-700 font-semibold hover:underline">
            Log in
          </Link>
        </div>
      </div>
    </div>
  );
};
