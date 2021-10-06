import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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
  public speaking: Observable<boolean>;

  constructor(private voice: VoiceService, private route: ActivatedRoute) { }
   

  async ngOnInit() {
    await this.voice.start();

    const user = this.route.snapshot.paramMap.get("userId");
    this.online = this.voice.online(user);
    this.speaking = this.voice.speaking(user);
  }

  async ngOnDestroy() {
    await this.voice.stop();
  }
}
