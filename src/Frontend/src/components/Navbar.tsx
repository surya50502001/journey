import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Compass, GitCompare, ListTodo, Bookmark, FileText, User, LogOut, Settings, Menu, X, Sparkles } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export const Navbar: React.FC = () => {
  const { user, logout, loginDemo } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const isActive = (path: string) => location.pathname === path;

  return (
    <header className="sticky top-0 z-50 bg-stone-50/90 backdrop-blur-md border-b border-stone-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        {/* Brand */}
        <Link to="/" className="flex items-center gap-2 group">
          <div className="w-8 h-8 rounded-lg bg-stone-900 text-stone-50 flex items-center justify-center font-bold text-lg shadow-sm group-hover:bg-brand-600 transition-colors">
            BJ
          </div>
          <div className="flex flex-col">
            <span className="font-semibold text-stone-950 tracking-tight text-base leading-none">
              Builder Journey
            </span>
            <span className="text-[10px] font-mono tracking-wider text-stone-500 uppercase mt-0.5">
              Intelligence Platform
            </span>
          </div>
        </Link>

        {/* Desktop Nav */}
        <nav className="hidden md:flex items-center gap-1">
          <Link
            to="/discover"
            className={`px-3 py-1.5 rounded-md text-sm font-medium transition-colors flex items-center gap-1.5 ${
              isActive('/discover')
                ? 'bg-stone-200 text-stone-900'
                : 'text-stone-600 hover:text-stone-950 hover:bg-stone-100'
            }`}
          >
            <Compass className="w-4 h-4 text-stone-500" />
            Discover Builders
          </Link>
          <Link
            to="/compare"
            className={`px-3 py-1.5 rounded-md text-sm font-medium transition-colors flex items-center gap-1.5 ${
              isActive('/compare')
                ? 'bg-stone-200 text-stone-900'
                : 'text-stone-600 hover:text-stone-950 hover:bg-stone-100'
            }`}
          >
            <GitCompare className="w-4 h-4 text-stone-500" />
            Comparator
          </Link>
          <Link
            to="/action-plan"
            className={`px-3 py-1.5 rounded-md text-sm font-medium transition-colors flex items-center gap-1.5 ${
              isActive('/action-plan')
                ? 'bg-stone-200 text-stone-900'
                : 'text-stone-600 hover:text-stone-950 hover:bg-stone-100'
            }`}
          >
            <ListTodo className="w-4 h-4 text-stone-500" />
            Action Plan
          </Link>
          <Link
            to="/saved"
            className={`px-3 py-1.5 rounded-md text-sm font-medium transition-colors flex items-center gap-1.5 ${
              isActive('/saved')
                ? 'bg-stone-200 text-stone-900'
                : 'text-stone-600 hover:text-stone-950 hover:bg-stone-100'
            }`}
          >
            <Bookmark className="w-4 h-4 text-stone-500" />
            Watchlist
          </Link>
        </nav>

        {/* User / CTA */}
        <div className="hidden md:flex items-center gap-3">
          {user ? (
            <div className="flex items-center gap-2">
              <Link
                to="/settings"
                className="flex items-center gap-2 px-3 py-1.5 rounded-md hover:bg-stone-100 text-stone-700 text-sm"
              >
                <div className="w-6 h-6 rounded-full bg-brand-100 text-brand-800 font-bold text-xs flex items-center justify-center border border-brand-300">
                  {user.fullName.charAt(0)}
                </div>
                <span className="font-medium text-xs text-stone-800 max-w-[100px] truncate">{user.fullName}</span>
              </Link>
              <button
                onClick={() => {
                  logout();
                  navigate('/');
                }}
                title="Log Out"
                className="p-1.5 text-stone-400 hover:text-stone-700 rounded-md hover:bg-stone-100"
              >
                <LogOut className="w-4 h-4" />
              </button>
            </div>
          ) : (
            <div className="flex items-center gap-2">
              <button
                onClick={async () => {
                  await loginDemo();
                  navigate('/discover');
                }}
                className="text-xs px-2.5 py-1 rounded bg-amber-50 text-amber-900 border border-amber-300 hover:bg-amber-100 font-medium flex items-center gap-1"
              >
                <Sparkles className="w-3.5 h-3.5 text-amber-600" />
                Demo 1-Click
              </button>
              <Link
                to="/login"
                className="text-sm font-medium text-stone-600 hover:text-stone-950 px-3 py-1.5"
              >
                Log In
              </Link>
              <Link
                to="/register"
                className="text-sm font-medium bg-stone-900 text-stone-50 px-3.5 py-1.5 rounded-md hover:bg-stone-800 transition-colors shadow-sm"
              >
                Get Started
              </Link>
            </div>
          )}
        </div>

        {/* Mobile menu button */}
        <div className="flex md:hidden items-center gap-2">
          <button
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            className="p-2 text-stone-600 rounded-md hover:bg-stone-100"
          >
            {mobileMenuOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
          </button>
        </div>
      </div>

      {/* Mobile Drawer */}
      {mobileMenuOpen && (
        <div className="md:hidden border-b border-stone-200 bg-stone-50 px-4 pt-2 pb-4 space-y-2">
          <Link
            to="/discover"
            onClick={() => setMobileMenuOpen(false)}
            className="block px-3 py-2 rounded-md text-sm font-medium text-stone-700 hover:bg-stone-200"
          >
            Discover Builders
          </Link>
          <Link
            to="/compare"
            onClick={() => setMobileMenuOpen(false)}
            className="block px-3 py-2 rounded-md text-sm font-medium text-stone-700 hover:bg-stone-200"
          >
            Comparator
          </Link>
          <Link
            to="/action-plan"
            onClick={() => setMobileMenuOpen(false)}
            className="block px-3 py-2 rounded-md text-sm font-medium text-stone-700 hover:bg-stone-200"
          >
            Action Plan
          </Link>
          <Link
            to="/saved"
            onClick={() => setMobileMenuOpen(false)}
            className="block px-3 py-2 rounded-md text-sm font-medium text-stone-700 hover:bg-stone-200"
          >
            Watchlist
          </Link>
          <Link
            to="/settings"
            onClick={() => setMobileMenuOpen(false)}
            className="block px-3 py-2 rounded-md text-sm font-medium text-stone-700 hover:bg-stone-200"
          >
            Settings & Disclosures
          </Link>

          {!user && (
            <div className="pt-2 border-t border-stone-200 flex flex-col gap-2">
              <button
                onClick={async () => {
                  await loginDemo();
                  setMobileMenuOpen(false);
                  navigate('/discover');
                }}
                className="w-full text-center text-xs py-2 rounded bg-amber-50 text-amber-900 border border-amber-300 font-medium"
              >
                Instant 1-Click Demo Login
              </button>
              <Link
                to="/login"
                onClick={() => setMobileMenuOpen(false)}
                className="block text-center text-sm py-2 text-stone-700 font-medium"
              >
                Log In
              </Link>
              <Link
                to="/register"
                onClick={() => setMobileMenuOpen(false)}
                className="block text-center text-sm py-2 bg-stone-900 text-stone-50 rounded-md font-medium"
              >
                Get Started
              </Link>
            </div>
          )}
        </div>
      )}
    </header>
  );
};
