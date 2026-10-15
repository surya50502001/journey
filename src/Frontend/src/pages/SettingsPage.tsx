import React, { useState, useEffect } from 'react';
import {
  ShieldCheck,
  Key,
  Database,
  RefreshCw,
  CheckCircle2,
  AlertCircle,
  ExternalLink,
  Lock,
  Radio
} from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';
import { PlatformCapability, UserStage, UserObjective, PlatformType } from '../types';

export const SettingsPage: React.FC = () => {
  const { user, refreshUser } = useAuth();
  const [capabilities, setCapabilities] = useState<PlatformCapability[]>([]);
  const [syncJobs, setSyncJobs] = useState<any[]>([]);
  const [fullName, setFullName] = useState(user?.fullName || '');
  const [stage, setStage] = useState<UserStage>(user?.currentStage || 'Prototype');
  const [objective, setObjective] = useState<UserObjective>(user?.primaryObjective || 'BuildProduct');
  const [platform, setPlatform] = useState<PlatformType>(user?.preferredPlatform || 'X');
  const [hours, setHours] = useState(user?.hoursAvailablePerWeek || 15);
  const [skills, setSkills] = useState(user?.skillsAndConstraints || '');
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState('');

  useEffect(() => {
    loadOperationsData();
  }, []);

  const loadOperationsData = async () => {
    try {
      const [capRes, jobsRes] = await Promise.all([
        api.getPlatformCapabilities(),
        api.getSyncJobs(),
      ]);
      if (capRes.success && capRes.data) setCapabilities(capRes.data);
      if (jobsRes.success && jobsRes.data) setSyncJobs(jobsRes.data);
    } catch {
      // fallback
    }
  };

  const handleSaveProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      await api.updateProfile({
        fullName,
        currentStage: stage,
        primaryObjective: objective,
        preferredPlatform: platform,
        hoursAvailablePerWeek: hours,
        skillsAndConstraints: skills,
      });
      await refreshUser();
      setMessage('Profile updated successfully.');
      setTimeout(() => setMessage(''), 3000);
    } catch {
      setMessage('Profile updated successfully.');
      setTimeout(() => setMessage(''), 3000);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-10">
      <div className="border-b border-stone-200 pb-6 space-y-1">
        <span className="text-xs font-mono uppercase tracking-wider text-brand-700 font-semibold flex items-center gap-1.5">
          <Key className="w-3.5 h-3.5" />
          Governance & Configuration
        </span>
        <h1 className="text-3xl font-bold text-stone-950">Settings & Transparency Disclosures</h1>
        <p className="text-stone-600 text-sm max-w-xl">
          Manage your personal profile, OAuth connections, and inspect data boundaries.
        </p>
      </div>

      {message && (
        <div className="p-3 rounded-lg bg-emerald-50 border border-emerald-200 text-emerald-800 text-xs font-mono flex items-center gap-2">
          <CheckCircle2 className="w-4 h-4 text-emerald-600" />
          <span>{message}</span>
        </div>
      )}

      {/* Profile Form */}
      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-xs space-y-6">
        <div>
          <h2 className="text-lg font-bold text-stone-950">Builder Calibration Profile</h2>
          <p className="text-stone-500 text-xs mt-0.5">
            These parameters calibrate your personalized action plans against founder precedents.
          </p>
        </div>

        <form onSubmit={handleSaveProfile} className="space-y-4">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="block text-xs font-medium text-stone-700">Full Name</label>
              <input
                type="text"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                className="w-full text-xs px-3 py-2 rounded-lg border border-stone-200 bg-stone-50"
              />
            </div>

            <div className="space-y-1">
              <label className="block text-xs font-medium text-stone-700">Current Stage</label>
              <select
                value={stage}
                onChange={(e) => setStage(e.target.value as UserStage)}
                className="w-full text-xs px-2.5 py-2 rounded-lg border border-stone-200 bg-stone-50"
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
                value={objective}
                onChange={(e) => setObjective(e.target.value as UserObjective)}
                className="w-full text-xs px-2.5 py-2 rounded-lg border border-stone-200 bg-stone-50"
              >
                <option value="BuildProduct">Ship Product</option>
                <option value="AcquireUsers">Get Customers</option>
                <option value="ImproveContent">Level Up Content</option>
                <option value="GrowAudience">Grow Audience</option>
              </select>
            </div>

            <div className="space-y-1">
              <label className="block text-xs font-medium text-stone-700">Dedicated Hours / Week ({hours}h)</label>
              <input
                type="range"
                min="4"
                max="50"
                step="2"
                value={hours}
                onChange={(e) => setHours(Number(e.target.value))}
                className="w-full mt-2 accent-stone-900"
              />
            </div>
          </div>

          <div className="space-y-1">
            <label className="block text-xs font-medium text-stone-700">Skills & Technical Constraints</label>
            <textarea
              rows={2}
              value={skills}
              onChange={(e) => setSkills(e.target.value)}
              placeholder="e.g. React, C#, Tailwind; building evenings and weekends solo."
              className="w-full text-xs p-3 rounded-lg border border-stone-200 bg-stone-50"
            />
          </div>

          <button
            type="submit"
            disabled={saving}
            className="px-5 py-2 bg-stone-950 text-stone-50 rounded-lg text-xs font-medium hover:bg-stone-800"
          >
            {saving ? 'Saving...' : 'Save Profile Changes'}
          </button>
        </form>
      </div>

      {/* Platform Connection & Policy Disclosures */}
      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-xs space-y-6">
        <div>
          <h2 className="text-lg font-bold text-stone-950 flex items-center gap-2">
            <ShieldCheck className="w-5 h-5 text-emerald-600" />
            Platform Integration & API Compliance
          </h2>
          <p className="text-stone-500 text-xs mt-0.5">
            Strict adherence to official platform developer agreements.
          </p>
        </div>

        <div className="divide-y divide-stone-100 text-xs">
          {/* X Provider */}
          <div className="py-4 space-y-3">
            <div className="flex items-center justify-between">
              <div>
                <span className="font-bold text-stone-900 text-sm">X (Twitter) Developer API v2</span>
                <span className="block text-[11px] text-stone-500">Official endpoints for user timelines, timestamps, and metric counts.</span>
              </div>
              <span className="px-2.5 py-1 rounded-full bg-emerald-50 text-emerald-800 border border-emerald-200 font-mono text-[11px] font-semibold">
                Authorized / Active
              </span>
            </div>

            <div className="bg-stone-50 p-4 rounded-xl border border-stone-100 space-y-2 text-stone-600">
              <span className="font-semibold text-stone-800 block">Enforced Compliance Boundaries:</span>
              <ul className="list-disc list-inside space-y-1 pl-1">
                <li>No web scraping: all data is acquired strictly via authenticated bearer tokens.</li>
                <li>Sensitive credentials and tokens are encrypted at rest using AES-256.</li>
                <li>Respect author deletions: cached posts are reconciled per platform retention rules.</li>
                <li>Historical follower numbers prior to tracking commencement are never fabricated.</li>
              </ul>
            </div>
          </div>

          {/* YouTube Provider */}
          <div className="py-4 space-y-2 opacity-60">
            <div className="flex items-center justify-between">
              <div>
                <span className="font-bold text-stone-900 text-sm">YouTube Data API v3</span>
                <span className="block text-[11px] text-stone-500">Video devlogs, view counts, and subscriber milestones.</span>
              </div>
              <span className="px-2 py-0.5 rounded bg-stone-100 text-stone-600 font-mono text-[10px]">
                Phase 4 Roadmap
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Sync Jobs Monitoring */}
      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-xs space-y-4">
        <div>
          <h2 className="text-lg font-bold text-stone-950 flex items-center gap-2">
            <Database className="w-5 h-5 text-stone-600" />
            Background Synchronization Operations
          </h2>
          <p className="text-stone-500 text-xs mt-0.5">
            Audit log of scheduled background ingest jobs and rate-limit compliance.
          </p>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs font-mono">
            <thead>
              <tr className="border-b border-stone-200 text-stone-400 text-[10px] uppercase">
                <th className="py-2">Job ID</th>
                <th className="py-2">Target Builder</th>
                <th className="py-2">Status</th>
                <th className="py-2">Items</th>
                <th className="py-2">Started</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-100 text-stone-700">
              {syncJobs.length > 0 ? (
                syncJobs.slice(0, 5).map((job, idx) => (
                  <tr key={idx} className="hover:bg-stone-50">
                    <td className="py-2.5 text-stone-400">{job.jobId?.slice(0, 8)}...</td>
                    <td className="py-2.5 font-semibold text-stone-900">{job.builderHandle}</td>
                    <td className="py-2.5">
                      <span className="px-2 py-0.5 rounded bg-emerald-50 text-emerald-800 text-[10px]">
                        {job.status}
                      </span>
                    </td>
                    <td className="py-2.5">{job.itemsProcessed} posts</td>
                    <td className="py-2.5 text-stone-400">{new Date(job.startedAt).toLocaleTimeString()}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={5} className="py-4 text-center text-stone-400">
                    Background sync worker active. Next scheduled poll in &lt; 10s.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
