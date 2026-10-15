import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Check, ArrowRight, Sparkles, Compass } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';
import { UserStage, UserObjective, PlatformType } from '../types';

export const OnboardingPage: React.FC = () => {
  const { user, refreshUser } = useAuth();
  const navigate = useNavigate();

  const [stage, setStage] = useState<UserStage>(user?.currentStage || 'Prototype');
  const [objective, setObjective] = useState<UserObjective>(user?.primaryObjective || 'BuildProduct');
  const [platform, setPlatform] = useState<PlatformType>('X');
  const [hours, setHours] = useState(user?.hoursAvailablePerWeek || 15);
  const [skills, setSkills] = useState('Fullstack TypeScript & React, minimal marketing experience');
  const [loading, setLoading] = useState(false);

  const handleFinish = async () => {
    setLoading(true);
    try {
      await api.updateProfile({
        fullName: user?.fullName || 'Builder',
        currentStage: stage,
        primaryObjective: objective,
        preferredPlatform: platform,
        hoursAvailablePerWeek: hours,
        skillsAndConstraints: skills,
      });

      // Generate initial action plan
      await api.generateActionPlan({
        stage,
        objective,
        hoursPerWeek: hours,
        skills,
        constraints: 'Limited time, building solo',
        targetPlatform: platform,
      });

      await refreshUser();
      navigate('/action-plan');
    } catch (err) {
      console.error(err);
      navigate('/discover');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto py-12 px-4 space-y-8">
      <div className="text-center space-y-2">
        <span className="text-xs font-mono uppercase tracking-wider text-brand-700 font-semibold">
          Step 1 of 1 • Profile Calibration
        </span>
        <h1 className="text-3xl font-bold text-stone-950">Tailor Your Builder Journey</h1>
        <p className="text-stone-600 text-sm max-w-lg mx-auto">
          We match your current situation against verified historical milestones from founders who built similar products.
        </p>
      </div>

      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-sm space-y-8">
        {/* Stage selection */}
        <div className="space-y-3">
          <label className="block text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold">
            Where are you today?
          </label>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            {[
              { id: 'Idea', label: 'Idea Validation', desc: 'Pre-code, validating demand' },
              { id: 'Prototype', label: 'Building Prototype', desc: 'Developing first interactive demo' },
              { id: 'Launched', label: 'Recently Launched', desc: 'Live product, seeking first users' },
              { id: 'FirstUsers', label: 'First Customers', desc: '1-100 paying customers, refining' },
              { id: 'Growth', label: 'Scaling & Growth', desc: 'Accelerating distribution & retention' }
            ].map((s) => (
              <button
                key={s.id}
                type="button"
                onClick={() => setStage(s.id as UserStage)}
                className={`p-4 rounded-xl border text-left transition-all ${
                  stage === s.id
                    ? 'border-stone-900 bg-stone-900 text-stone-50 shadow-sm'
                    : 'border-stone-200 bg-stone-50/50 hover:bg-stone-100 text-stone-800'
                }`}
              >
                <div className="font-semibold text-sm">{s.label}</div>
                <div className={`text-xs mt-1 ${stage === s.id ? 'text-stone-300' : 'text-stone-500'}`}>
                  {s.desc}
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Objective selection */}
        <div className="space-y-3">
          <label className="block text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold">
            Primary Goal for the Next 60 Days
          </label>
          <div className="grid grid-cols-2 gap-3">
            {[
              { id: 'BuildProduct', title: 'Ship the Product', desc: 'Deliver an interactive, functional version' },
              { id: 'AcquireUsers', title: 'Get Paying Customers', desc: 'Convert early audience to paid subscribers' },
              { id: 'ImproveContent', title: 'Level Up Content DNA', desc: 'Publish consistent build-in-public formats' },
              { id: 'GrowAudience', title: 'Grow Builder Trust', desc: 'Accumulate genuine follower momentum' },
            ].map((obj) => (
              <button
                key={obj.id}
                type="button"
                onClick={() => setObjective(obj.id as UserObjective)}
                className={`p-3.5 rounded-xl border text-left transition-all ${
                  objective === obj.id
                    ? 'border-brand-600 bg-brand-50 text-brand-950 font-medium'
                    : 'border-stone-200 bg-stone-50/50 hover:bg-stone-100 text-stone-800'
                }`}
              >
                <div className="text-xs font-semibold">{obj.title}</div>
                <div className="text-[11px] text-stone-500 mt-0.5">{obj.desc}</div>
              </button>
            ))}
          </div>
        </div>

        {/* Target Platform */}
        <div className="space-y-2">
          <label className="block text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold">
            Primary Audience Distribution Platform
          </label>
          <div className="flex gap-3">
            {['X', 'YouTube', 'LinkedIn'].map((p) => (
              <button
                key={p}
                type="button"
                onClick={() => setPlatform(p as PlatformType)}
                className={`px-4 py-2 rounded-lg text-xs font-medium border transition-colors ${
                  platform === p
                    ? 'bg-stone-900 text-stone-50 border-stone-900'
                    : 'bg-stone-50 border-stone-200 text-stone-700 hover:bg-stone-100'
                }`}
              >
                {p}
              </button>
            ))}
          </div>
        </div>

        {/* Weekly Capacity */}
        <div className="space-y-2">
          <div className="flex justify-between items-center text-xs">
            <span className="font-mono uppercase tracking-wider text-stone-500 font-semibold">
              Weekly Dedicated Time
            </span>
            <span className="font-mono font-bold text-stone-900">{hours} hours / week</span>
          </div>
          <input
            type="range"
            min="4"
            max="50"
            step="2"
            value={hours}
            onChange={(e) => setHours(Number(e.target.value))}
            className="w-full accent-stone-900"
          />
        </div>

        {/* Submit */}
        <div className="pt-4 border-t border-stone-100 flex items-center justify-between">
          <button
            onClick={() => navigate('/discover')}
            className="text-xs text-stone-500 hover:text-stone-900 font-medium"
          >
            Skip for now
          </button>

          <button
            onClick={handleFinish}
            disabled={loading}
            className="px-6 py-2.5 bg-stone-950 text-stone-50 font-medium text-sm rounded-lg hover:bg-stone-800 flex items-center gap-2"
          >
            {loading ? 'Synthesizing...' : 'Generate Roadmap'}
            <ArrowRight className="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  );
};
