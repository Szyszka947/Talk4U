import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { off, send } from 'process';
import * as SimplePeer from 'simple-peer';
import { TargetPeer } from '../../models/target-peer';
import { ToastService } from '../../services/alert/toast.service';

@Component({
  selector: 'app-chat-room',
  templateUrl: './chat-room.component.html',
  styleUrls: ['./chat-room.component.css']
})
export class ChatRoomComponent implements OnInit {

  constructor(private _toastService: ToastService) { }

  connection = new HubConnectionBuilder()
    .withUrl('/signalingHub')
    .withAutomaticReconnect()
    .build();

  peer!: SimplePeer.Instance;
  targetPeer!: SimplePeer.Instance;

  @ViewChild('targetPeerVideo')
  targetPeerVideo!: ElementRef<HTMLVideoElement>;

  @ViewChild('myVideo')
  myVideo!: ElementRef<HTMLVideoElement>;

  ngOnInit(): void {
    this.connection.start();

    this.connection.on('ReceivedConnectionRequest', async (targetPeer: TargetPeer, offer: string) => {
      navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
      }).then((mediaStream) => {
        mediaStream.getVideoTracks()[0].applyConstraints(
          {
            frameRate: 60
          });

        this.targetPeer = new SimplePeer({
          initiator: false,
          trickle: true,
          stream: mediaStream
        });

        if ('srcObject' in this.targetPeerVideo.nativeElement) {
          this.myVideo.nativeElement.srcObject = mediaStream;
        } else {
          this.myVideo.nativeElement.src = window.URL.createObjectURL(mediaStream);
        }
        this.myVideo.nativeElement.play();

        this.targetPeer.signal(offer);

        this.targetPeer.on('signal', (answer) => {
          this.connection.invoke('SendAnswerAsync', targetPeer, JSON.stringify(answer));
        });

        this.targetPeer.on('stream', (mediaStream: MediaStream) => {
          if ('srcObject' in this.targetPeerVideo.nativeElement) {
            this.targetPeerVideo.nativeElement.srcObject = mediaStream;
          } else {
            this.targetPeerVideo.nativeElement.src = window.URL.createObjectURL(mediaStream);
          }

          this.targetPeerVideo.nativeElement.play();

        });

        this.targetPeer.on('connect', () => {
          this.connection.invoke('ImNotAvailable');
        });

        this.targetPeer.on('close', () => {
          this.connection.invoke('ImAvailable');
        });
      }).catch((err) => {
        this._toastService.toast('error', err + ' (Camera or microphone)');
        this.connection.invoke('SendNoMediaDevicesAnswerAsync', targetPeer);
      });
    });

    this.connection.on('ReceivedConnectionRequestAnswer', (answer) => {
      this.peer.signal(answer);
    });

    this.connection.on('ReceivedTargetPeerNoMediaAnswer', () => {
      this._toastService.toast('info', "Randomly selected talker has no required media devices. Try again.");
    });

    this.connection.on('ReceivedTerminateConnectionRequest', () => {
      this.targetPeer.destroy();
      this.nextPeer();
    });
  }

  async nextPeer() {
    const newPeer: TargetPeer = await this.connection.invoke('GetNextPeerToCall');

    if (newPeer == null) return this._toastService.toast('error', 'There is no free talker. Try again later!');

    if (this.peer != undefined) {
      this.peer.destroy();
    }

    navigator.mediaDevices.getUserMedia({
      video: true,
      audio: true
    }).then((mediaStream) => {
      mediaStream.getVideoTracks()[0].applyConstraints(
        {
          frameRate: 60
        });

      this.peer = new SimplePeer({
        initiator: true,
        trickle: true,
        stream: mediaStream
      });

      if ('srcObject' in this.targetPeerVideo.nativeElement) {
        this.myVideo.nativeElement.srcObject = mediaStream;
      } else {
        this.myVideo.nativeElement.src = window.URL.createObjectURL(mediaStream);
      }
      this.myVideo.nativeElement.play();

      this.peer.on('signal', (offer) => {
        this.connection.invoke('SendOfferAsync', newPeer, JSON.stringify(offer));
      });

      this.peer.on('stream', (mediaStream: MediaStream) => {
        if ('srcObject' in this.targetPeerVideo.nativeElement) {
          this.targetPeerVideo.nativeElement.srcObject = mediaStream;
        } else {
          this.targetPeerVideo.nativeElement.src = window.URL.createObjectURL(mediaStream);
        }

        this.targetPeerVideo.nativeElement.play();

      });

      this.peer.on('connect', () => {
        this.connection.invoke('ImNotAvailable');
      });

      this.peer.on('close', () => {
        this.connection.invoke('ImAvailable');
      });
    }).catch((err) => {
      this._toastService.toast('error', err + ' (Camera or microphone)');
    });
  }
}
