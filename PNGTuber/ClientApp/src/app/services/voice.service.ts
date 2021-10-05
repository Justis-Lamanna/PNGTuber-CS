import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, ISubscription } from "@microsoft/signalr";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VoiceService {

  private connection: HubConnection

  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl("/voices")
      .build();
  }

  async start(): Promise<void> {
    return this.connection.start();
  }

  async stop(): Promise<void> {
    return this.connection.stop();
  }

  online(user_id: string): Observable<boolean> {
    return new Observable(subscriber => {
      const res = this.connection.stream("Online", user_id)
        .subscribe({
          next: (item) => subscriber.next(item),
          error: (err) => subscriber.error(err),
          complete: () => subscriber.complete()
        });
      subscriber.add(() => res.dispose());
    });
  }
}
