import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { DASHBOARD_MOCK } from '../data/dashboard.mock';
import { DashboardData } from '../models/dashboard.models';

/**
 * Data access service for the dashboard feature.
 *
 * Attempts to load the dashboard view model from the backend and falls back to local
 * mock data when the API cannot be reached.
 */
@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  /** Angular HttpClient used for backend dashboard calls. */
  private readonly http = inject(HttpClient);

  /** Base URL of the local ASP.NET Core API during development. */
  private readonly apiBaseUrl = 'http://localhost:5187';

  /**
   * Loads the dashboard data used by all dashboard widgets.
   *
   * @returns Observable that emits API data when available, otherwise the local mock
   * dashboard with a fallback health message.
   */
  getDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.apiBaseUrl}/api/dashboard`).pipe(
      map((dashboard) => ({
        ...dashboard,
        healthMessage: dashboard.healthMessage || 'ASP.NET Core API connected',
      })),
      catchError(() =>
        of({
          ...DASHBOARD_MOCK,
          healthMessage: 'Szerver nem érhető el, helyi mock adatok láthatók',
        }),
      ),
    );
  }
}
