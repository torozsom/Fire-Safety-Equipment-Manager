import { DashboardData } from '../models/dashboard.models';

/**
 * Local dashboard fixture used while the ASP.NET Core API is unavailable.
 *
 * The data follows the same contract as the production dashboard endpoint so the UI can
 * render consistently in development and during service fallback.
 */
export const DASHBOARD_MOCK: DashboardData = {
  healthMessage: 'Mock adatok betöltve',
  period: '2026. 05. 20. - 2026. 06. 20.',
  metrics: [
    {
      title: 'Lejárt eszközök',
      value: 12,
      subtitle: 'Összesen 248 eszközből',
      tone: 'danger',
      trend: [12, 11, 13, 12, 15, 14, 16, 16, 18, 17, 19],
    },
    {
      title: 'Közelgő lejáratok (30 napon belül)',
      value: 36,
      subtitle: 'Figyelmet igénylő eszköz',
      tone: 'warning',
      trend: [34, 33, 35, 32, 31, 34, 36, 33, 32, 35, 36],
    },
    {
      title: 'Rendben lévő eszközök',
      value: 200,
      subtitle: 'Az összes eszköz 80,6%-a',
      tone: 'success',
      trend: [190, 194, 201, 199, 193, 194, 200, 196, 202, 198, 200],
    },
    {
      title: 'Hibás eszközök',
      value: 8,
      subtitle: 'Javítást igényel',
      tone: 'info',
      trend: [6, 7, 8, 8, 9, 8, 6, 6, 7, 6, 7],
    },
  ],
  statusSummary: [
    { label: 'Rendben', value: 200, percentage: 80.6, color: 'green' },
    { label: 'Figyelmeztetés', value: 36, percentage: 14.5, color: 'orange' },
    { label: 'Lejárt', value: 12, percentage: 4.8, color: 'red' },
    { label: 'Hibás', value: 8, percentage: 3.1, color: 'violet' },
  ],
  expiringEquipment: [
    { id: 'PO-2023-0012', location: 'Irodaépület - 1. emelet', daysLeft: 14 },
    { id: 'TC-2022-0005', location: 'Raktár csarnok', daysLeft: 14 },
    { id: 'PO-2023-0018', location: 'Üzlethelyiség - Raktár', daysLeft: 7 },
    { id: 'CO2-2022-0003', location: 'Adatközpont', daysLeft: 1 },
  ],
  expiredEquipment: [
    {
      id: 'PO-2023-0001',
      type: 'Porral oltó',
      site: 'Irodaépület - 2. emelet',
      dueDate: '2026. 05. 18.',
      status: 'Lejárt',
    },
    {
      id: 'TC-2022-0002',
      type: 'Tűzcsap',
      site: 'Raktár csarnok',
      dueDate: '2026. 05. 17.',
      status: 'Lejárt',
    },
    {
      id: 'CO2-2021-0004',
      type: 'CO2 oltó',
      site: 'Adatközpont',
      dueDate: '2026. 05. 16.',
      status: 'Lejárt',
    },
    {
      id: 'PO-2022-0011',
      type: 'Porral oltó',
      site: 'Üzlethelyiség',
      dueDate: '2026. 05. 15.',
      status: 'Lejárt',
    },
  ],
  activities: [
    { title: 'Karbantartás rögzítve', target: 'PO-2023-0007', time: 'ma 09:42', tone: 'success' },
    {
      title: 'Hibabejelentés létrehozva',
      target: 'TC-2021-0002',
      time: 'ma 08:15',
      tone: 'danger',
    },
    { title: 'Eszköz frissítve', target: 'CO2-2022-0004', time: 'tegnap 16:21', tone: 'info' },
    { title: 'Munkalap letöltve', target: 'PO-2023-0003', time: 'tegnap 14:03', tone: 'violet' },
  ],
};
