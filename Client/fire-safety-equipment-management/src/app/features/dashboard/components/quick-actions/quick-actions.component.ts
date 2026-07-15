import { Component } from '@angular/core';
import { IconComponent, IconName } from '../../../../shared/icon/icon.component';

/**
 * Static quick-action definition rendered by the dashboard action panel.
 */
interface QuickAction {
  /** Localized action label shown on the button. */
  readonly label: string;

  /** Icon displayed next to the action label. */
  readonly icon: IconName;
}

/**
 * Dashboard panel containing the primary operational shortcuts.
 */
@Component({
  selector: 'app-quick-actions',
  imports: [IconComponent],
  templateUrl: './quick-actions.component.html',
  styleUrl: './quick-actions.component.scss',
})
export class QuickActionsComponent {
  /** Ordered list of shortcut buttons displayed in the quick actions panel. */
  readonly quickActions: QuickAction[] = [
    { label: 'Új eszköz rögzítése', icon: 'plus' },
    { label: 'Hibabejelentés létrehozása', icon: 'alert' },
    { label: 'Karbantartás rögzítése', icon: 'calendar' },
    { label: 'Munkalap létrehozása', icon: 'file' },
    { label: 'QR kód beolvasása', icon: 'qr' },
  ];
}
