import { Component, Input } from '@angular/core';

/**
 * Names of inline SVG icons supported by the shared icon component.
 */
export type IconName =
  | 'alert'
  | 'bell'
  | 'box'
  | 'building'
  | 'calendar'
  | 'chart'
  | 'clipboard'
  | 'edit'
  | 'eye'
  | 'file'
  | 'flame'
  | 'home'
  | 'logout'
  | 'mail'
  | 'menu'
  | 'pin'
  | 'plus'
  | 'qr'
  | 'search'
  | 'settings'
  | 'users';

/**
 * SVG path definitions keyed by icon name.
 *
 * Each path is designed for the shared icon component viewport and is rendered by the
 * template as a single stroked path.
 */
const ICON_PATHS: Record<IconName, string> = {
  alert:
    'M12 9v4m0 4h.01M10.3 4.4 2.6 17.7A2 2 0 0 0 4.3 21h15.4a2 2 0 0 0 1.7-3.3L13.7 4.4a2 2 0 0 0-3.4 0Z',
  bell: 'M18 8a6 6 0 1 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9Zm-3 13a3 3 0 0 1-6 0',
  box: 'm21 16-9 5-9-5V8l9-5 9 5v8Zm-18-8 9 5 9-5M12 13v8',
  building:
    'M4 21V5a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v16M3 21h18M8 7h4M8 11h4M8 15h4M18 9h1a1 1 0 0 1 1 1v11',
  calendar:
    'M8 2v4m8-4v4M3 10h18M5 4h14a2 2 0 0 1 2 2v13a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2Z',
  chart: 'M4 19V9m5 10V5m5 14v-7m5 7V3',
  clipboard:
    'M9 4h6M9 4a3 3 0 0 1 6 0M9 4H7a2 2 0 0 0-2 2v13a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2h-2',
  edit: 'm12 20 9-9-4-4-9 9-1 5 5-1ZM16 7l1-1a2.8 2.8 0 0 1 4 4l-1 1',
  eye: 'M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6S2 12 2 12Zm10 3a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z',
  file: 'M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8l-6-6Zm0 0v6h6M8 13h8M8 17h5',
  flame: 'M12 22c4 0 7-3 7-7 0-3-2-5-4-7 .1 2-1 3-2 4 .2-4-2-7-5-10 1 4-3 6-3 10 0 6 4 10 7 10Z',
  home: 'M3 11 12 3l9 8v10h-6v-6H9v6H3V11Z',
  logout: 'M10 17 15 12l-5-5M15 12H3M21 3v18h-6',
  mail: 'M4 4h16a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2Zm0 3 8 6 8-6',
  menu: 'M4 7h16M4 12h16M4 17h16',
  pin: 'M12 21s7-5 7-12a7 7 0 1 0-14 0c0 7 7 12 7 12Zm0-9a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z',
  plus: 'M12 5v14M5 12h14',
  qr: 'M4 4h6v6H4V4Zm10 0h6v6h-6V4ZM4 14h6v6H4v-6Zm10 0h2m4 0v2m-6 4h2m2-4h2v4h-4',
  search: 'M21 21 16.7 16.7M10.5 18a7.5 7.5 0 1 1 0-15 7.5 7.5 0 0 1 0 15Z',
  settings:
    'M12 15a3 3 0 1 0 0-6 3 3 0 0 0 0 6Zm8-3a8 8 0 0 0-.1-1l2-1.6-2-3.4-2.4 1a8 8 0 0 0-1.7-1L15.5 3h-4l-.4 3a8 8 0 0 0-1.7 1L7 6 5 9.4 7 11a8 8 0 0 0 0 2l-2 1.6L7 18l2.4-1a8 8 0 0 0 1.7 1l.4 3h4l.4-3a8 8 0 0 0 1.7-1l2.4 1 2-3.4-2-1.6c.1-.3.1-.7.1-1Z',
  users:
    'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2m7-10a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm13 10v-2a4 4 0 0 0-3-3.9M16 3.1a4 4 0 0 1 0 7.8',
};

/**
 * Lightweight inline SVG icon component used across dashboard widgets.
 */
@Component({
  selector: 'app-icon',
  templateUrl: './icon.component.html',
  styleUrl: './icon.component.scss',
})
export class IconComponent {
  /** Icon identifier mapped to a path in the local icon registry. */
  @Input({ required: true }) name!: IconName;

  /**
   * Resolved SVG path data for the selected icon.
   */
  get path(): string {
    return ICON_PATHS[this.name];
  }
}
