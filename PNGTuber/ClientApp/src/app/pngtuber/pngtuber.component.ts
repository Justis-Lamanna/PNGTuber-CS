import { Component, OnDestroy, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, ISubscription } from "@microsoft/signalr";
import { Observable } from 'rxjs';
import { VoiceService } from '../services/voice.service';

@Component({
  selector: 'app-pngtuber',
  templateUrl: './pngtuber.component.html',
  styleUrls: ['./pngtuber.component.css']
})
export class PNGTuberComponent implements OnInit, OnDestroy {

  public online: Observable<boolean>;

  constructor(private voice: VoiceService) { }
   

  async ngOnInit() {
    await this.voice.start();

    this.online = this.voice.online("248612704019808258");
  }

  async ngOnDestroy() {
    await this.voice.stop();
  }
}
