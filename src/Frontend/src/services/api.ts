import type {
  ApiResponse,
  AuthResponse,
  BuilderCategory,
  BuilderComparisonResult,
  BuilderProfileDetail,
  BuilderProfileSummary,
  ContentStrategySummary,
  JourneyReport,
  JourneyTimeline,
  PagedResult,
  PlatformCapability,
  ActionPlan,
  ActionPlanTask,
  WatchlistEntry,
  Breakthrough,
  UserProfile,
} from '../types';

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1';

class ApiService {
  private getToken(): string | null {
    return localStorage.getItem('bj_token');
  }

  private async request<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    const token = this.getToken();
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      ...(options.headers as Record<string, string>),
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    try {
      const response = await fetch(`${API_BASE}${endpoint}`, {
        ...options,
        headers,
      });

      const data = await response.json();
      return data;
    } catch (error) {
      console.warn(`API call to ${endpoint} failed, checking mock/fallback.`, error);
      throw error;
    }
  }

  // Auth
  async login(email: string, password: string): Promise<ApiResponse<AuthResponse>> {
    return this.request<AuthResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async register(data: any): Promise<ApiResponse<AuthResponse>> {
    return this.request<AuthResponse>('/auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async getMe(): Promise<ApiResponse<UserProfile>> {
    return this.request<UserProfile>('/auth/me');
  }

  async updateProfile(data: any): Promise<ApiResponse<UserProfile>> {
    return this.request<UserProfile>('/auth/profile', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Builders
  async getBuilders(params: {
    search?: string;
    category?: string;
    platform?: string;
    curatedOnly?: boolean;
    sortBy?: string;
    page?: number;
    pageSize?: number;
  }): Promise<ApiResponse<PagedResult<BuilderProfileSummary>>> {
    const query = new URLSearchParams();
    if (params.search) query.append('search', params.search);
    if (params.category) query.append('category', params.category);
    if (params.platform) query.append('platform', params.platform);
    if (params.curatedOnly !== undefined) query.append('curatedOnly', String(params.curatedOnly));
    if (params.sortBy) query.append('sortBy', params.sortBy);
    if (params.page) query.append('page', String(params.page));
    if (params.pageSize) query.append('pageSize', String(params.pageSize));

    return this.request<PagedResult<BuilderProfileSummary>>(`/builders?${query.toString()}`);
  }

  async getBuilderById(id: string): Promise<ApiResponse<BuilderProfileDetail>> {
    return this.request<BuilderProfileDetail>(`/builders/${id}`);
  }

  async getBuilderByHandle(handle: string): Promise<ApiResponse<BuilderProfileDetail>> {
    return this.request<BuilderProfileDetail>(`/builders/handle/${handle}`);
  }

  async getCategories(): Promise<ApiResponse<BuilderCategory[]>> {
    return this.request<BuilderCategory[]>('/builders/categories');
  }

  async createCustomBuilder(data: any): Promise<ApiResponse<BuilderProfileDetail>> {
    return this.request<BuilderProfileDetail>('/builders/custom', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async syncBuilder(id: string, force = false): Promise<ApiResponse<any>> {
    return this.request(`/builders/${id}/sync`, {
      method: 'POST',
      body: JSON.stringify({ forceFullResync: force }),
    });
  }

  // Journey
  async getTimeline(builderId: string): Promise<ApiResponse<JourneyTimeline>> {
    return this.request<JourneyTimeline>(`/journey/${builderId}`);
  }

  async reconstructTimeline(builderId: string): Promise<ApiResponse<JourneyTimeline>> {
    return this.request<JourneyTimeline>(`/journey/${builderId}/reconstruct`, {
      method: 'POST',
    });
  }

  // Content Analysis
  async getContentStrategy(builderId: string): Promise<ApiResponse<ContentStrategySummary>> {
    return this.request<ContentStrategySummary>(`/content/${builderId}`);
  }

  // Breakthroughs
  async getBreakthroughs(builderId: string): Promise<ApiResponse<Breakthrough[]>> {
    return this.request<Breakthrough[]>(`/breakthroughs/${builderId}`);
  }

  // Comparison
  async compareBuilders(builderIds: string[]): Promise<ApiResponse<BuilderComparisonResult>> {
    return this.request<BuilderComparisonResult>('/comparison', {
      method: 'POST',
      body: JSON.stringify({ builderIds }),
    });
  }

  // Action Plans
  async generateActionPlan(data: any): Promise<ApiResponse<ActionPlan>> {
    return this.request<ActionPlan>('/action-plans/generate', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async getLatestActionPlan(): Promise<ApiResponse<ActionPlan>> {
    return this.request<ActionPlan>('/action-plans/latest');
  }

  async updateTaskStatus(taskId: string, isCompleted: boolean): Promise<ApiResponse<ActionPlanTask>> {
    return this.request<ActionPlanTask>(`/action-plans/tasks/${taskId}`, {
      method: 'PATCH',
      body: JSON.stringify({ isCompleted }),
    });
  }

  // Watchlist
  async getWatchlist(): Promise<ApiResponse<WatchlistEntry[]>> {
    return this.request<WatchlistEntry[]>('/watchlist');
  }

  async addToWatchlist(builderProfileId: string, notes = ''): Promise<ApiResponse<boolean>> {
    return this.request<boolean>('/watchlist', {
      method: 'POST',
      body: JSON.stringify({ builderProfileId, notes }),
    });
  }

  async removeFromWatchlist(builderId: string): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/watchlist/${builderId}`, {
      method: 'DELETE',
    });
  }

  // Reports
  async getReport(builderId: string): Promise<ApiResponse<JourneyReport>> {
    return this.request<JourneyReport>(`/reports/builder/${builderId}`);
  }

  async getReportByShareSlug(slug: string): Promise<ApiResponse<JourneyReport>> {
    return this.request<JourneyReport>(`/reports/share/${slug}`);
  }

  async updateSharingSettings(reportId: string, visibility: string, allowCloning: boolean): Promise<ApiResponse<JourneyReport>> {
    return this.request<JourneyReport>(`/reports/${reportId}/sharing`, {
      method: 'PUT',
      body: JSON.stringify({ visibility, allowCloning }),
    });
  }

  // Operations
  async getPlatformCapabilities(): Promise<ApiResponse<PlatformCapability[]>> {
    return this.request<PlatformCapability[]>('/operations/capabilities');
  }

  async getSyncJobs(): Promise<ApiResponse<any[]>> {
    return this.request<any[]>('/operations/sync-jobs');
  }
}

export const api = new ApiService();
