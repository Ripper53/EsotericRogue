using System;
using System.Collections.Generic;

namespace DungeonRogue {
    public class NoteManager {
        private readonly Random rng = new Random();
        private readonly List<NoteTracker> availableNotes;
        private readonly List<ListTracker> notes;

        private class NoteTracker {
            public readonly Note Note;
            public readonly ListTracker ListTracker;

            public NoteTracker(Note note, ListTracker listTracker) {
                Note = note;
                ListTracker = listTracker;
            }
        }

        private class ListTracker {
            public IReadOnlyList<Note> Notes;
            public int Index;

            public ListTracker(IReadOnlyList<Note> notes) {
                Notes = notes;
                Index = 0;
            }

            public Note GetNote() {
                Index++;
                if (Index < Notes.Count)
                    return Notes[Index];
                else
                    return null;
            }
        }

        public NoteManager() {
            availableNotes = new List<NoteTracker>();
            notes = new List<ListTracker>();
        }

        public void AddNote(Note note) {
            availableNotes.Add(new NoteTracker(note, null));
        }

        public void AddNotes(IReadOnlyList<Note> notes) {
            ListTracker listTracker = new ListTracker(notes);
            availableNotes.Add(new NoteTracker(notes[0], listTracker));
            this.notes.Add(new ListTracker(notes));
        }

        public Note GetNote() {
            if (availableNotes.Count > 0) {
                int index = rng.Next(availableNotes.Count);
                NoteTracker noteTracker = availableNotes[index];
                ListTracker listTracker = noteTracker.ListTracker;
                if (listTracker != null) {
                    Note nextNote = listTracker.GetNote();
                    if (nextNote != null) {
                        availableNotes[index] = new NoteTracker(nextNote, listTracker);
                    } else {
                        availableNotes.RemoveAt(index);
                    }
                } else {
                    availableNotes.RemoveAt(index);
                }
                return noteTracker.Note;
            }
            return null;
        }

    }
}
