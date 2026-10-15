import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Sparkles, Lock, ArrowRight, AlertCircle } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login, loginDemo } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const success = await login(email, password);
      if (success) {
        navigate('/discover');
      } else {
        setError('Invalid email or password. Please try again.');
      }
    } catch {
      setError('An error occurred. Try the 1-Click Demo button below.');
    } finally {
      setLoading(false);
    }
  };

  const handleDemoClick = async () => {
    setLoading(true);
    await loginDemo();
    navigate('/discover');
  };

  return (
    <div className="max-w-md mx-auto py-16 px-4">
      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-sm space-y-6">
        <div className="text-center space-y-1">
          <div className="w-10 h-10 rounded-xl bg-stone-900 text-stone-50 flex items-center justify-center font-bold text-lg mx-auto mb-3">
            BJ
          </div>
          <h1 className="text-2xl font-bold text-stone-950">Welcome back</h1>
          <p className="text-stone-500 text-xs">
            Access evidence-based builder journeys & personal action plans
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

          <button
            type="submit"
            disabled={loading}
            className="w-full py-2.5 bg-stone-950 text-stone-50 font-medium rounded-lg hover:bg-stone-800 transition-colors text-sm flex items-center justify-center gap-2"
          >
            {loading ? 'Authenticating...' : 'Sign In'}
            <ArrowRight className="w-4 h-4" />
          </button>
        </form>

        <div className="relative py-2">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-stone-200" />
          </div>
          <div className="relative flex justify-center text-[11px] uppercase font-mono">
            <span className="bg-white px-2 text-stone-400">Or use instant access</span>
          </div>
        </div>

        <button
          onClick={handleDemoClick}
          className="w-full py-2.5 bg-amber-50 border border-amber-300 text-amber-950 rounded-lg hover:bg-amber-100 transition-colors text-sm font-medium flex items-center justify-center gap-2"
        >
          <Sparkles className="w-4 h-4 text-amber-600" />
          1-Click Instant Demo Login
        </button>

        <div className="text-center text-xs text-stone-500 pt-2 border-t border-stone-100">
          Don't have an account?{' '}
          <Link to="/register" className="text-brand-700 font-semibold hover:underline">
            Register here
          </Link>
        </div>
      </div>
    </div>
  );
};
